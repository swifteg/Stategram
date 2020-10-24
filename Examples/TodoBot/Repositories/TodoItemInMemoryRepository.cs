using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodoBot.Entities;

namespace TodoBot.Repositories
{
    class TodoItemInMemoryRepository : ITodoItemRepository
    {

        private readonly Dictionary<int, List<TodoItem>> _userToItems = new Dictionary<int, List<TodoItem>>();
        private readonly Dictionary<int, TodoItem> _idToItem = new Dictionary<int, TodoItem>();
        private int _autoIncrement = 0;
        
        public Task DeleteByIdAsync(int id)
        {
            if(_idToItem.Remove(id, out TodoItem item))
            {
                var userId = item.TelegramUserId;
                _userToItems[userId].Remove(item);
            }
            else
            {
                throw new ItemNotFoundException();
            }
            return Task.CompletedTask;
        }

        public Task<IEnumerable<TodoItem>> GetAllFromUserAsync(int userTelegramId)
        {
            if (!_userToItems.ContainsKey(userTelegramId))
                throw new UserNotFoundException();
            return Task.FromResult((IEnumerable<TodoItem>)_userToItems[userTelegramId]);
        }

        public Task<TodoItem> GetItemByIdAsync(int id)
        {
            if (!_idToItem.ContainsKey(id))
                throw new ItemNotFoundException();
            return Task.FromResult(_idToItem[id]);
        }

        public Task SaveAsync(TodoItem item)
        {
            item.Id = _autoIncrement++;
            _idToItem[item.Id] = item;
            if (!_userToItems.ContainsKey(item.TelegramUserId))
                _userToItems[item.TelegramUserId] = new List<TodoItem>();
            _userToItems[item.TelegramUserId].Add(item);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(TodoItem item)
        {
            _idToItem[item.Id] = item;
            return Task.CompletedTask;
        }
    }
}
