using System;
using System.Reflection;
using Stategram.Attributes;
using Stategram.Extensions;
using Stategram.Fluent;
using Stategram.Middleware;

namespace Stategram
{
    public partial class Router
    {
        public interface IRouterConfig
        {
            /// <summary>
            /// Fluent syntax. Chain Add.
            /// </summary>
            /// <param name="forState">Source state for which to add transitions.</param>
            /// <returns></returns>
            IRegisterTransitions RegisterOuterTransitionsFrom(Type fromState);

            /// <summary>
            /// Fluent syntax. Chain AddImplementation and AddInstance.
            /// </summary>
            /// <returns></returns>
            IRegisterDependencies RegisterDependencies();

            /// <summary>
            /// Fluent syntax. Chain Add().
            /// </summary>
            /// <returns></returns>
            IRegisterMiddleware RegisterMessageMiddleware();

            /// <summary>
            /// Fluent syntax. Chain Start() and then Add().
            /// </summary>
            /// <returns></returns>
            IRegisterStartController RegisterControllers();
        }

        class RouterConfigurator : IRouterConfig,
            IRegisterDependencies,
            IRegisterTransitions,
            IRegisterMiddleware,
            IRegisterStartController,
            IRegisterController
        {
            private readonly Router _router;

            // set in RegisterTransitionsFrom
            private Type _fromState;
            private Type _startOuterState;
            private string _startInnerState;

            public RouterConfigurator(Router router)
            {
                _router = router;
            }

            #region middleware

            public IRegisterMiddleware RegisterMessageMiddleware()
            {
                return this;
            }

            public IRegisterMiddleware Add(Type middleware)
            {
                if (!typeof(IMiddleware).IsAssignableFrom(middleware))
                {
                    throw new Exception($"{middleware.Name} does not implement {nameof(IMiddleware)}");
                }
                _router._middlewares.Add(middleware);
                _router._container.Register(middleware);
                return this;
            }

            #endregion

            #region transitions

            public IRegisterTransitions RegisterOuterTransitionsFrom(Type fromState)
            {
                _fromState = fromState;
                // TODO: considering using the full name of the controller instead of mapping shorthands to types for consistency
                _startOuterState = _router._controllerNameToType[_router._startState.OuterState];
                _startInnerState = _router._startState.InnerState;
                return this;
            }

            private IRegisterTransitions AddTransition(object symbol, Type outerState, string innerState, bool forward)
            {
                MethodInfo method;
                try
                {
                    method = outerState.GetMethod(innerState);
                }
                catch
                {
                    throw new Exception($"{outerState.GetType()} does not have a method called {innerState}");
                }

                if (!method.IsEntry())
                {
                    throw new Exception($"{innerState} – only methods marked with {nameof(EntryAttribute)} can be the destination of an outer transition.");
                }

                _router._stateMachine.AddTransition(_fromState, symbol, outerState, innerState, forward);
                return this;
            }

            public IRegisterTransitions ToStart(object symbol)
            {
                return AddTransition(symbol, _startOuterState, _startInnerState, false);
            }

            public IRegisterTransitions AddInstant(object symbol, Type outerState, string innerState)
            {
                return AddTransition(symbol, outerState, innerState, true);
            }

            public IRegisterTransitions AddDeferred(object symbol, Type outerState, string innerState)
            {
                return AddTransition(symbol, outerState, innerState, false);
            }

            #endregion

            #region dependencies

            public IRegisterDependencies RegisterDependencies()
            {
                return this;
            }

            public IRegisterDependencies AddImplementation(Type service, Type implementation)
            {
                _router._container.Register(service, implementation);
                return this;
            }

            public IRegisterDependencies AddImplementation(Type service)
            {
                _router._container.Register(service, service);
                return this;
            }

            public IRegisterDependencies AddInstance(Type service, object instance)
            {
                _router._container.RegisterInstance(service, instance);
                return this;
            }

            public IRegisterDependencies AddSingleton(Type service, Type implementation)
            {
                _router._container.Register(service, implementation, SimpleInjector.Lifestyle.Singleton);
                return this;
            }

            #endregion

            #region controllers

            public IRegisterStartController RegisterControllers()
            {
                return this;
            }

            public IRegisterController Start(Type outerState, string innerState)
            {
                MethodInfo method;
                try
                {
                    method = outerState.GetMethod(innerState);
                }
                catch
                {
                    throw new Exception($"{outerState.GetType()} does not have a method called {innerState}");
                }

                if (!method.IsEntry())
                {
                    throw new Exception($"{innerState} – only methods marked with {nameof(EntryAttribute)} can be the start state.");
                }

                _router._stateMachine.SetCurrentState(outerState);
                _router._startState.OuterState = outerState.GetControllerName();
                _router._startState.InnerState = innerState;
                ((IRegisterController)this).Add(outerState);
                return this;
            }

            IRegisterController IRegisterController.Add(Type controllerType)
            {
                _router._container.Register(controllerType);
                var controllerName = controllerType.GetControllerName();
                _router._controllerNameToType[controllerName] = controllerType;
                _router._stateMachine.AddState(controllerType);
                return this;
            }

            #endregion
        }
    }
}
