using Stategram.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Stategram.Middleware
{
    public class ResetMiddlewareSettings
    {
        public ResetMiddlewareSettings(Type targetOuterState, string targetInnerState, string resetCommand, string message = null)
        {
            TargetOuterState = targetOuterState;
            TargetInnerState = targetInnerState;
            ResetCommand = resetCommand;
            Message = message;
        }

        public Type TargetOuterState { get; }
        public string TargetInnerState { get; }
        public string ResetCommand { get; }
        public string Message { get; }
    }

    public class ResetMiddleware : IMiddleware
    {
        public string Command { get; private set; }
        public Type TargetOuterState { get; private set; }
        public string TargetInnerState { get; private set; }

        public string ResponseText { get; private set; }

        public ResetMiddleware(ResetMiddlewareSettings settings)
        {
            Command = settings.ResetCommand;
            TargetOuterState = settings.TargetOuterState;
            TargetInnerState = settings.TargetInnerState;
            ResponseText = settings.Message;
        }

        public Task<bool> Pipe(MiddlewareContext context)
        {
            if(context.ControllerContext?.Message?.Text == Command)
            {
                context.UserState.InnerState = TargetInnerState;
                context.UserState.OuterState = TargetOuterState.GetControllerName();
                if (ResponseText != null)
                {
                    context.Response.Add(new ResponseMessage()
                    {
                        Message = ResponseText
                    }); 
                }
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
    }
}
