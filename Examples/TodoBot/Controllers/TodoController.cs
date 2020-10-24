using SimpleInjector;
using Stategram;
using Stategram.Attributes;
using Stategram.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using TodoBot.Entities;
using TodoBot.Repositories;
using TodoBot.Services;
using Stategram.Utils;

namespace TodoBot.Controllers
{
    class TodoController : BaseController
    {
        public enum Results
        {
            TodoItemAdded,
            TodoListShown
        }

        private readonly ITodoItemRepository _itemRepository;
        private readonly UIItemService _itemService;

        public TodoController(ITodoItemRepository itemRepository, UIItemService itemService)
        {
            _itemRepository = itemRepository;
            _itemService = itemService;
        }

        public async Task<Transition> AcceptItemText()
        {
            var newToDoItem = new TodoItem() { 
                TelegramUserId = Context.TelegramUserId,
                Text = Context.Message.Text
            };
            await _itemRepository.SaveAsync(newToDoItem);
            return Transition.Outer(Results.TodoItemAdded).WithMessage("New task added!");
        }

        [Entry]
        public Task<Transition> AddTodo()
        {
            return Task.FromResult(Transition.Inner(AcceptItemText).WithMessage("What is it you need to do?"));
        }

        [Entry]
        public async Task<Transition> ListTodo()
        {
            IEnumerable<TodoItem> todoItems = null;
            try
            {
                todoItems = await _itemRepository.GetAllFromUserAsync(Context.TelegramUserId);
            }
            catch { }

            if(todoItems == null || !todoItems.Any())
                return Transition.Outer(Results.TodoListShown).WithMessage("You have not added any tasks yet!");

            var responses = todoItems.Select(todo => 
                    UIItemService.FormatItem(todo, FormatCallback(TickTask, todo.Id), FormatCallback(RemoveTask, todo.Id)));

            return Transition.Outer(Results.TodoListShown).WithMessages(responses);           
        }

        /// <summary>
        /// Payload is the id of the ticked task.
        /// </summary>
        [StatelessListener]
        public async Task TickTask()
        {
            var todoItemId = int.Parse(Context.CallbackQuery.ExtractCallbackPayload());
            var item = await _itemRepository.GetItemByIdAsync(todoItemId);
            if (item.Finished)
                return;
            item.Finished = true;
            await _itemRepository.UpdateAsync(item);
            await _itemService.TickMessage(Context.Message);
        }

        /// <summary>
        /// Payload is the id of the removed task.
        /// </summary>
        [StatelessListener]
        public async Task RemoveTask()
        {
            var todoItemId = int.Parse(Context.CallbackQuery.ExtractCallbackPayload());
            await _itemRepository.DeleteByIdAsync(todoItemId);
            await _itemService.RemoveItem(Context.Message);
        }
    }
}
