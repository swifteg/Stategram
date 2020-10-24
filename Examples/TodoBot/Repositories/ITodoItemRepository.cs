using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TodoBot.Entities;

namespace TodoBot.Repositories
{
    interface ITodoItemRepository
    {
        Task<TodoItem> GetItemByIdAsync(int id);
        Task<IEnumerable<TodoItem>> GetAllFromUserAsync(int userTelegramId);
        Task DeleteByIdAsync(int id);
        Task SaveAsync(TodoItem item);
        Task UpdateAsync(TodoItem item);
    }
}
