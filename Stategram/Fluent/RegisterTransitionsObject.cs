using Stategram.Attributes;
using Stategram.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stategram.Fluent
{
    public class RegisterTransitionsObject
    {
        private readonly OuterStateMachine<Type> _stateMachine;
        private readonly Type _state;
        private readonly Type _startOuterState;
        private readonly string _startInnerState;

        public RegisterTransitionsObject(OuterStateMachine<Type> stateMachine, Type state, Type startOuterState, string startInnerState)
        {
            _stateMachine = stateMachine;
            _state = state;
            _startOuterState = startOuterState;
            _startInnerState = startInnerState;
        }

        private RegisterTransitionsObject Add(object symbol, Type outerState, string innerState, bool forward)
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

            if(!method.IsEntry())
            {
                throw new Exception($"{innerState} – only methods marked with {nameof(EntryAttribute)} can be the destination of an outer transition.");
            }

            _stateMachine.AddTransition(_state, symbol, outerState, innerState, forward);
            return this;
        }

        public RegisterTransitionsObject AddDeferred(object symbol, Type outerState, string innerState)
        {
            return Add(symbol, outerState, innerState, false);
        }

        public RegisterTransitionsObject AddInstant(object symbol, Type outerState, string innerState)
        {
            return Add(symbol, outerState, innerState, true);
        }

        public RegisterTransitionsObject ToStart(object symbol)
        {
            return Add(symbol, _startOuterState, _startInnerState, false);
        }

    }
}
