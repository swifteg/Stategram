using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stategram
{
    public class StateMachine<StateType>
    {
        private readonly HashSet<StateType> _states = new();
        private readonly Dictionary<(StateType state, object transition), StateType> _transitions = new();
        private StateType _currentState;

        public void AddState(StateType state)
        {
            _states.Add(state);
        }

        public int GetStateCount()
        {
            return _states.Count;
        }

        public StateType GetCurrentState()
        {
            return _currentState;
        }

        public void AddTransition(StateType from, object symbol, StateType to)
        {
            if(!_states.Contains(from))
            {
                throw new Exception($"{nameof(from)} has not been added as a state. Call {nameof(AddState)} first.");
            }

            if (!_states.Contains(to))
            {
                throw new Exception($"{nameof(to)} has not been added as a state. Call {nameof(AddState)} first.");
            }

            _transitions[(from, symbol)] = to;
        }

        public void SetCurrentState(StateType currentState)
        {
            _currentState = currentState;
        }

        public StateType Step(object symbol)
        {
            _currentState = _transitions[(_currentState, symbol)];
            return _currentState;
        }

        public StateType IdempotentStep(StateType from, object symbol)
        {
            return _transitions[(from, symbol)];
        }
    }

    public class OuterStateMachine<StateType> 
    {
        private readonly StateMachine<StateType> _stateMachine = new();
        private readonly Dictionary<(StateType outerState, object symbol), (string innerState, bool forward)> _innerStateDict = new();

        public void AddState(StateType state)
        {
            _stateMachine.AddState(state);
        }

        public void AddTransition(StateType from, object symbol, StateType outerState, string innerState, bool forward)
        {
            _innerStateDict[(from, symbol)] = (innerState, forward);
            _stateMachine.AddTransition(from, symbol, outerState);
        }

        public void SetCurrentState(StateType state)
        {
            _stateMachine.SetCurrentState(state);
        }

        public int GetStateCount()
        {
            return _stateMachine.GetStateCount();
        }

        public (StateType outerState, string innerState, bool forward) Step(object symbol)
        {
            var currentState = _stateMachine.GetCurrentState();
            (var newInnerState, var forward) = _innerStateDict[(currentState, symbol)];
            var newOuterState = _stateMachine.Step(symbol);
            return (newOuterState, newInnerState, forward);
        }

        public (StateType outerState, string innerState, bool forward) IdempotentStep(StateType from, object symbol)
        {
            var newOuterState = _stateMachine.IdempotentStep(from, symbol);
            (var newInnerState, var forward) = _innerStateDict[(from, symbol)];
            return (newOuterState, newInnerState, forward);
        }

    }


}
