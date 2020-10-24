using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram
{
    public class UserState : IUserState
    {
        public string OuterState { get; set; }
        public string InnerState { get; set; }

        public UserState(string outerState, string innerState)
        {
            OuterState = outerState;
            InnerState = innerState;
        }
    }
}
