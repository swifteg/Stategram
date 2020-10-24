using SimpleInjector;
using System;
using System.Collections.Generic;
using Stategram.Extensions;
using System.Reflection;
using Stategram.Attributes;

namespace Stategram.Fluent
{

    public class RegisterStartStateObject
    {
        private readonly Container _container;
        private readonly OuterStateMachine<Type> _stateMachine;
        private readonly Dictionary<string, Type> _controllerNameToType;
        private readonly UserState _startState;


        public RegisterStartStateObject(Container container,
            Dictionary<string, Type> nameToTypeDict,
            OuterStateMachine<Type> stateMachine,
            UserState startStateRef)
        {
            _container = container;
            _controllerNameToType = nameToTypeDict;
            _stateMachine = stateMachine;
            _startState = startStateRef;
        }

        public RegisterControllersObject Start(Type outerState, string innerState)
        {
            // TODO: validate method signature
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

            _stateMachine.SetCurrentState(outerState);
            _startState.OuterState = outerState.GetControllerName();
            _startState.InnerState = innerState;
            var regControllers = new RegisterControllersObject(_container, _controllerNameToType, _stateMachine);
            regControllers.Add(outerState);
            return regControllers;
        }
    }

    public class RegisterControllersObject
    {
        private readonly Container _container;
        private readonly OuterStateMachine<Type> _stateMachine;
        private readonly Dictionary<string, Type> _controllerNameToType;

        public RegisterControllersObject(Container container, Dictionary<string, Type> nameToTypeDict, OuterStateMachine<Type> stateMachine)
        {
            _container = container;
            _controllerNameToType = nameToTypeDict;
            _stateMachine = stateMachine;
        }
        public RegisterControllersObject Add(Type controllerType)
        {
            _container.Register(controllerType);
            var controllerName = controllerType.GetControllerName();
            _controllerNameToType[controllerName] = controllerType;

            _stateMachine.AddState(controllerType);
            if (_stateMachine.GetStateCount() == 1)
                _stateMachine.SetCurrentState(controllerType);
            return this;
        }
    }
}
