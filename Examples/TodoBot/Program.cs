using Stategram;
using System;
using TodoBot.Controllers;
using TodoBot.Repositories;
using TodoBot.Services;

namespace TodoBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var apiKey = args[0];
            var router = new Router(apiKey);
            router.RegisterControllers()
                .Start(typeof(StartController), nameof(StartController.Welcome))
                .Add(typeof(TodoController));

            router.RegisterDependencies()
                .AddSingleton(typeof(ITodoItemRepository), typeof(TodoItemInMemoryRepository))
                .AddImplementation(typeof(UIItemService));

            router.RegisterOuterTransitionsFrom(typeof(StartController))
                .AddInstant(StartController.Results.ListPressed, typeof(TodoController), nameof(TodoController.ListTodo))
                .AddInstant(StartController.Results.TodoPressed, typeof(TodoController), nameof(TodoController.AddTodo));

            router.RegisterOuterTransitionsFrom(typeof(TodoController))
                .ToStart(TodoController.Results.TodoListShown)
                .ToStart(TodoController.Results.TodoItemAdded);

            router.Start();
            Console.ReadKey();
        }
    }
}
