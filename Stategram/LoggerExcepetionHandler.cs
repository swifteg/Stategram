using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stategram
{
    class LoggerExcepetionHandler : IExceptionHandler
    {

        private readonly ILogger _logger;
        public LoggerExcepetionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public Task Handle(Exception e)
        {
            _logger.LogError($"[Exception] {e.GetType().Name}, {e.Message}");
            return Task.CompletedTask;
        }
    }
}
