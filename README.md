# Stategram


Stategram is a framework for bulding telegram bots using a state machine. 

Some of the features:
  - ASP.NET-inspired
  - Asynchronous
  - Fluent syntax
  - Dependency injection
  - Middleware
  - Overridable interfaces

## Quick start

The repo is at a very early stage, so please refer to the Examples/ for now.
**Contributions are very welcome**

## Core concepts

### State management
The user's state is made up of the **outer state** and the **inner state**.
The **outer state** specifies the controller that will handle the next user's request.
The **inner state** specifies the method inside the controller that will handle the next user's request.

Controllers are decoupled from eachother; while the inner state can be easily switched within a single controller, transitions between different controllers are configured externally. This allows for rapid development of new features while keeping the code well-isolated and maintanable. 

Stateful request handling methods have a signature `Task<Transition> MethodName()`. The returned `Transition` object specifies how the state is changed. 
  - `Transition.Stay()` retains the state until the next request.
  - `Transition.Inner(NextMethod)` switches the inner state.
  - `Transition.Outer(ReturnedSymbol)` specifies the symbol that is produced by the outer state, which gets picked up by the externally configured state machine.

### To be continued...


## Roadmap

 - Documentation
 - Documenting code
 - Testing utilities
 - More features
 - More example projects

## License
MIT