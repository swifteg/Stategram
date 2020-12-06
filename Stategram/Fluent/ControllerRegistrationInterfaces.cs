using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    interface IRegisterStartController
    {
        IRegisterController Start(Type outerState, string innerState);
    }

    interface IRegisterController
    {
        IRegisterController Add(Type outerState);
    }
}
