using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Stategram
{
    public class Transition
    {
        public delegate Task<Transition> StateMethod();
        public IEnumerable<ResponseMessage> Response { get; private set; }  
        public object Symbol { get; private set; }
        public StateMethod InnerTransition { get; private set; }
        public bool KeepState { get; private set; } = false;

        private readonly List<ResponseMessage> _responseMessages;

        private Transition() {
            _responseMessages = new List<ResponseMessage>();
            Response = _responseMessages;
        }

        public static Transition Outer(object symbol)
        {
            return new Transition()
            {
                Symbol = symbol
            };
        }

        public static Transition Stay()
        {
            return new Transition()
            {
                KeepState = true
            };
        }

        public static Transition Inner(StateMethod method)
        {
            return new Transition()
            {
                InnerTransition = method
            };
        }

        public Transition WithMessage(string text,
            int replyTo = 0,
            ResponseMessage replyToResponseMessage = null,
            IEnumerable<string> images = null,
            IEnumerable<IEnumerable<InlineKeyboardButton>> inlineKeyboard = null,
            IEnumerable<IEnumerable<string>> keyboard = null)
        {
            var responseMessage = new ResponseMessage()
            {
                Message = text,
                Images = images,
                ReplyToId = replyTo,
                InlineKeyboardButtons = inlineKeyboard,
                ReplyKerboardButtons = keyboard,
                ReplyToResponseMessage = replyToResponseMessage
            };

            _responseMessages.Add(responseMessage);

            return this;
        }

        public Transition WithMessages(IEnumerable<string> messages)
        {
            messages.ToList().ForEach(m => WithMessage(m));
            return this;
        }


        public Transition WithMessages(IEnumerable<ResponseMessage> messages)
        {
            _responseMessages.AddRange(messages);
            return this;
        }
    }
}
