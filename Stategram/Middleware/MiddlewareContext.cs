using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace Stategram.Middleware
{
    public class MiddlewareContext
    {
        public IUserState UserState { get; }
        public ControllerContext ControllerContext { get; }
        public List<ResponseMessage> Response { get; }

        public MiddlewareContext(IUserState userState, ControllerContext context)
        {
            UserState = userState;
            ControllerContext = context;
            Response = new List<ResponseMessage>();
        }
    }
}
