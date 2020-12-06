using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    interface IRegisterTransitions
    {
        IRegisterTransitions AddDeferred(object symbol, Type outerState, string innerState);

        IRegisterTransitions AddInstant(object symbol, Type outerState, string innerState);

        IRegisterTransitions ToStart(object symbol);
    }
}
