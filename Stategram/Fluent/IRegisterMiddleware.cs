using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Fluent
{
    public interface IRegisterMiddleware
    {
        IRegisterMiddleware Add(Type middleware);
    }
}
