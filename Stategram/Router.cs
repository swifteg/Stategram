using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Stategram.Extensions;
using Stategram.Middleware;
using Stategram.Attributes;
using Stategram.Fluent;
using Telegram.Bot.Types.ReplyMarkups;

namespace Stategram
{
    public partial class Router
    {
        private readonly Container _container = new Container();

        private readonly ITelegramBotClient _bot;
        private readonly IStateRepository _stateRepository;
        private readonly ILogger _logger;
        private readonly IExceptionHandler _exceptionHandler;

        private readonly Dictionary<string, Type> _controllerNameToType = new Dictionary<string, Type>();
        private readonly OuterStateMachine<Type> _stateMachine = new OuterStateMachine<Type>();
        private readonly List<Type> _middlewares = new List<Type>();

        private readonly UserState _startState = new UserState(null, null);

        public Router(ITelegramBotClient bot,
            IStateRepository stateRepository = null,
            ILogger logger = null,
            IExceptionHandler exceptionHandler = null)
        {
            _bot = bot;
            _stateRepository = stateRepository ?? new InMemoryStateRepository();
            _logger = logger ?? new ConsoleLogger();
            _exceptionHandler = exceptionHandler ?? new LoggerExcepetionHandler(_logger);
            _container.RegisterInstance(typeof(ITelegramBotClient), bot);
        }

        public Router(string telegramApiKey, 
            IStateRepository stateRepository = null,
            ILogger logger = null,
            IExceptionHandler exceptionHandler = null) : this(new TelegramBotClient(telegramApiKey), stateRepository, logger, exceptionHandler)
        { }


        public void Start()
        {
            _container.Verify();
            _bot.OnMessage += OnMessage;
            _bot.OnCallbackQuery += OnCallbackQuery;
            _bot.StartReceiving();
        }

        public void Configure(Action<IRouterConfig> applyConfigurations)
        {
            applyConfigurations(new RouterConfigurator(this));
        }

        private async Task<bool> ApplyMessageMiddleware(MiddlewareContext context)
        {
            foreach(var mwareType in _middlewares)
            {
                var mware = (IMiddleware)_container.GetInstance(mwareType);
                if (!await mware.Pipe(context).ConfigureAwait(false))
                    return false;
            }
            return true;
        }

        private UserState CreateNewUserState() => new UserState(_startState.OuterState, _startState.InnerState);

        private async Task<bool> AdvanceState(IUserState userState, Transition transition)
        {
            if (transition.KeepState)
                return false;

            if (transition.InnerTransition != null)
            {
                userState.InnerState = transition.InnerTransition.GetMethodInfo().Name;
                return false;
            }

            if(transition.Symbol != null)
            {
                var currentOuterState = _controllerNameToType[userState.OuterState];
                try
                {
                    (var outerState, var innerState, var forward) = _stateMachine.IdempotentStep(currentOuterState, transition.Symbol);
                    userState.InnerState = innerState;
                    userState.OuterState = outerState.GetControllerName();
                    return forward;
                }
                catch
                {
                    await _logger.LogError($"There is no registered transition {transition.Symbol} from {currentOuterState.Name}")
                        .ConfigureAwait(false);
                    return false;
                }
            }

            return false;
        }

        private async Task ExecuteStatelessListener(ControllerContext statelessContext)
        {
            (var controllerName, var methodName) = statelessContext.CallbackQuery.ExtractSerializedListener();
            var listenerControllerType = _controllerNameToType[controllerName];
            var listenerMethod = listenerControllerType.GetMethod(methodName);
            if (listenerMethod.GetCustomAttribute<StatelessListenerAttribute>() == null)
            {
                // method is not a listener
                await _logger.LogError($"{methodName} is not marked with {nameof(StatelessListenerAttribute)}.");
                return;
            }

            var listenerController = (BaseController)_container.GetInstance(listenerControllerType);
            listenerController.Context = statelessContext;
            await ((Task)listenerMethod.Invoke(listenerController, null)).ConfigureAwait(false);
            return;
        }

        private async Task RouteContext(IUserState userState, ControllerContext context)
        {
            var callbackQuery = context.CallbackQuery;
            var message = context.Message;

            var middlewareContext = new MiddlewareContext(userState, context);
            var messageHasPassedMiddleware = await ApplyMessageMiddleware(middlewareContext).ConfigureAwait(false);
            if (!messageHasPassedMiddleware)
            {
                // middleware may have changed the state and set a response
                await _stateRepository.SetUserState(context.TelegramUserId, userState).ConfigureAwait(false);
                await SendResponses(context.ChatId, middlewareContext.Response).ConfigureAwait(false);
                return;
            }

            if (callbackQuery != null && callbackQuery.IsStateless())
            {
                try
                {
                    await ExecuteStatelessListener(context).ConfigureAwait(false);
                }
                catch(Exception e)
                {
                    await _exceptionHandler.Handle(e).ConfigureAwait(false);
                }
                return;
            }


            var controllerType = _controllerNameToType[userState.OuterState];
            var method = controllerType.GetMethod(userState.InnerState);

            if (callbackQuery != null && !method.AcceptsCallbacks())
            {
                await _logger.LogError($"{method.Name} is not marked with {nameof(AcceptCallbacksAttribute)}.").ConfigureAwait(false);
                return;
            }

            // Forward context to controllers
            bool forwardToNewState;
            do
            {
                controllerType = _controllerNameToType[userState.OuterState];
                method = controllerType.GetMethod(userState.InnerState);

                var controller = (BaseController)_container.GetInstance(controllerType);
                controller.Context = context;
                Transition transition;
                try
                {
                    var task = (Task<Transition>)method.Invoke(controller, null);
                    transition = await task.ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await _exceptionHandler.Handle(e).ConfigureAwait(false);
                    return;
                }

                forwardToNewState = await AdvanceState(userState, transition).ConfigureAwait(false);

                await _stateRepository.SetUserState(message.From.Id, userState).ConfigureAwait(false);
                await SendResponses(message.Chat.Id, transition.Response).ConfigureAwait(false);
            } while (forwardToNewState);
        }

        private async Task<IUserState> GetOrCreateUserState(User telegramUser)
        {
            var userState = await _stateRepository.GetUserState(telegramUser.Id).ConfigureAwait(false);
            if (userState == null)
                userState = CreateNewUserState();
            return userState;
        }
        private async void OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var userState = await GetOrCreateUserState(e.Message.From).ConfigureAwait(false);
            var context = new ControllerContext
            {
                TelegramUserId = e.Message.From.Id,
                Message = e.Message,
                Event = EventTypes.OnMessage,
                ChatId = e.Message.Chat.Id
            };

            await RouteContext(userState, context).ConfigureAwait(false);
        }

        private async void OnCallbackQuery(object sender, Telegram.Bot.Args.CallbackQueryEventArgs e)
        {
            var userState = await GetOrCreateUserState(e.CallbackQuery.From).ConfigureAwait(false);

            var context = new ControllerContext
            {
                TelegramUserId = e.CallbackQuery.From.Id,
                CallbackQuery = e.CallbackQuery,
                Message = e.CallbackQuery.Message,
                Event = EventTypes.OnCallbackQuery,
                ChatId = e.CallbackQuery.Message.Chat.Id
            };

            await RouteContext(userState, context).ConfigureAwait(false);
        }

        private async Task<int> SendResponse(long chatId,
            ResponseMessage response,
            IReadOnlyDictionary<ResponseMessage, int> prevResponseId)
        {
            if (response == null)
                return 0;

            if (response.ReplyToResponseMessage != null)
            {
                prevResponseId.TryGetValue(response.ReplyToResponseMessage, out int id);
                response.ReplyToId = id;
            }

            if (response.Images != null && response.Images.Any())
            {
                var mediaGroup = response.Images.Select(id => new InputMediaPhoto(new InputMedia(id))).ToList();
                if (response.Message != null)
                    mediaGroup[0].Caption = response.Message;

                var sentMessages = await _bot
                    .SendMediaGroupAsync(mediaGroup, chatId, replyToMessageId: response.ReplyToId)
                    .ConfigureAwait(false);
                return sentMessages.LastOrDefault()?.MessageId ?? 0;
            }
            else if (!string.IsNullOrEmpty(response.Message))
            {
                IReplyMarkup replyMarkup = null;

                if (response.ReplyKerboardButtons != null)
                {
                    var buttons = response.ReplyKerboardButtons.Select(row => row.Select(button => new KeyboardButton(button)));
                    replyMarkup = new ReplyKeyboardMarkup(buttons) { OneTimeKeyboard = true };
                }
                else if (response.InlineKeyboardButtons != null)
                {
                    //var buttons = response.InlineKeyboardButtons
                    //    .Select(row => row.Select(button => new InlineKeyboardButton() { Text = button, CallbackData = button }));
                    replyMarkup = new InlineKeyboardMarkup(response.InlineKeyboardButtons);
                }


                var sentMessage = await _bot
                    .SendTextMessageAsync(chatId, response.Message, replyToMessageId: response.ReplyToId, replyMarkup: replyMarkup)
                    .ConfigureAwait(false);
                return sentMessage?.MessageId ?? 0;
            }

            return 0;
        }

        private async Task SendResponses(long chatId, IEnumerable<ResponseMessage> responses)
        {
            var prevResponseId = new Dictionary<ResponseMessage, int>();
            foreach(var response in responses)
            {
                var sentResponseId = await SendResponse(chatId, response, prevResponseId).ConfigureAwait(false);
                if (sentResponseId != 0)
                {
                    prevResponseId[response] = sentResponseId;
                }
            }
        }
        
    }
}

