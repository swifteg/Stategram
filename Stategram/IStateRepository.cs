using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Stategram
{
    public interface IStateRepository
    {
        Task<IUserState> GetUserState(int telegramUserId);
        Task SetUserState(int telegramUserId, IUserState state);
    }
}
