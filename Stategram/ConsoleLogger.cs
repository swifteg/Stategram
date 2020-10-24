using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stategram
{
    internal class ConsoleLogger : ILogger
    {
        public Task Log(string log)
        {
            Console.WriteLine($"[{DateTime.Now:f}] {log}");
            return Task.CompletedTask;
        }

        public Task LogError(string log)
        {
            Console.WriteLine($"[{DateTime.Now:f}] [Error] {log}");
            return Task.CompletedTask;
        }
    }
}
