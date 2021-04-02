using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    public interface IRegisterStartController
    {
        IRegisterController Start(Type outerState, string innerState);
    }

    public interface IRegisterController
    {
        IRegisterController Add(Type controllerType);
    }
}
