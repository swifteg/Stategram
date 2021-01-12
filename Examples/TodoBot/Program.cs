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

            router.Configure(config =>
            {
                config.RegisterControllers()
                    .Start(typeof(StartController), nameof(StartController.Welcome))
                    .Add(typeof(TodoController));

                config.RegisterDependencies()
                    .AddSingleton(typeof(ITodoItemRepository), typeof(TodoItemInMemoryRepository))
                    .AddImplementation(typeof(UIItemService));

                config.RegisterOuterTransitionsFrom(typeof(StartController))
                    .AddInstant(StartController.Results.ListPressed, typeof(TodoController), nameof(TodoController.ListTodo))
                    .AddInstant(StartController.Results.TodoPressed, typeof(TodoController), nameof(TodoController.AddTodo));

                config.RegisterOuterTransitionsFrom(typeof(TodoController))
                    .ToStart(TodoController.Results.TodoListShown)
                    .ToStart(TodoController.Results.TodoItemAdded);
            });

            router.Start();
            Console.ReadKey();
        }
    }
}
