using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    interface IRegisterDependencies
    {
        IRegisterDependencies AddImplementation(Type service, Type implementation);

        IRegisterDependencies AddSingleton(Type service, Type implementation);

        IRegisterDependencies AddImplementation(Type service);

        IRegisterDependencies AddInstance(Type service, object instance);
    }
}
