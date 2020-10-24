using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Stategram.Middleware
{
    public interface IMiddleware
    {
        Task<bool> Pipe(MiddlewareContext context);
    }
}
