using Stategram;
using Stategram.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TodoBot.Controllers
{
    class StartController : BaseController
    {
        public enum Results
        {
            ListPressed,
            TodoPressed
        }

        public static string[][] Menu => new string[][] { new[] { "/list" }, new[] { "/todo" } };

        [Entry]
        public Task<Transition> Welcome()
        {
            var transition = Context.Message?.Text switch
            {
                "/start" => Transition.Stay().WithMessage("Here is the menu.", keyboard: Menu),
                "/list" => Transition.Outer(Results.ListPressed),
                "/todo" => Transition.Outer(Results.TodoPressed),
                _ => Transition.Stay().WithMessage("Say /start to see the menu.")
            };

            return Task.FromResult(transition);
        }
    }
}
