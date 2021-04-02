using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Stategram
{
    public class InMemoryStateRepository : IStateRepository
    {
        private readonly Dictionary<int, IUserState> _memory = new();

        public Task<IUserState> GetUserState(int telegramUserId)
        {
            _memory.TryGetValue(telegramUserId, out IUserState userState);
            return Task.FromResult(userState);
        }

        public Task SetUserState(int telegramUserId, IUserState state)
        {
            _memory[telegramUserId] = state;
            return Task.CompletedTask;
        }
    }
}
