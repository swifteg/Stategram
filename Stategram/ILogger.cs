using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stategram
{
    public interface ILogger
    {
        Task Log(string log);

        Task LogError(string log);
    }
}
