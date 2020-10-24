using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    public class RegisterDependenciesObject
    {
        private readonly Container _container;
        public RegisterDependenciesObject(Container container)
        {
            _container = container;
        }

        public RegisterDependenciesObject AddImplementation(Type service, Type implementation)
        {
            _container.Register(service, implementation);
            return this;
        }

        public RegisterDependenciesObject AddImplementation(Type service)
        {
            _container.Register(service, service);
            return this;
        }

        public RegisterDependenciesObject AddInstance(Type service, object instance)
        {
            _container.RegisterInstance(service, instance);
            return this;
        }

        public RegisterDependenciesObject AddSingleton(Type service, Type implementation)
        {
            _container.Register(service, implementation, Lifestyle.Singleton);
            return this;
        }
    }
}
