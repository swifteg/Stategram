using SimpleInjector;
using Stategram.Middleware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    public class RegisterMiddlewareObject
    {
        private readonly List<Type> _middlewares;
        private readonly Container _container;

        public RegisterMiddlewareObject(List<Type> messageMiddlewares, Container container)
        {
            _middlewares = messageMiddlewares;
            _container = container;
        }

        public RegisterMiddlewareObject Add(Type middleware)
        {
            if (!typeof(IMiddleware).IsAssignableFrom(middleware))
            {
                throw new Exception($"{middleware.Name} does not implement {nameof(IMiddleware)}");
            }
            _middlewares.Add(middleware);
            _container.Register(middleware);
            return this;
        }
    }
}
