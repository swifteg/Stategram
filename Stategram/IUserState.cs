using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram
{
    public interface IUserState
    {
        string OuterState { get; set; }
        string InnerState { get; set; }
    }
}
