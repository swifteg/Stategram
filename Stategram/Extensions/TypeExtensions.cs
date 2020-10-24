using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetControllerName(this Type controller)
        {
            if (controller.Name.Length < 10 && controller.Name[^10..] != "Controller")
                throw new Exception("Controller's name must end with \"Controller\"");
            return controller.Name[0..^10].ToLower();
        }
    }
}
