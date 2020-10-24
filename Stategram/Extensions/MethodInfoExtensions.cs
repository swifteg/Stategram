using Stategram.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stategram.Extensions
{
    public static class MethodInfoExtensions
    {
        public static bool AcceptsCallbacks(this MethodInfo method)
        {
            return method.GetCustomAttribute<AcceptCallbacksAttribute>() != null;
        }

        public static bool IsEntry(this MethodInfo method)
        {
            return method.GetCustomAttribute<EntryAttribute>() != null;
        }
    }
}
