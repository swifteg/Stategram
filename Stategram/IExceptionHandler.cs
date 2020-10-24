using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stategram
{
    public interface IExceptionHandler
    {
        Task Handle(Exception e);
    }
}
