using Stategram.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stategram.Utils
{
    public static class Callback
    {
        public static string GetAddress(Type outerAddress, string innerAddress)
        {
            return $"{outerAddress.GetControllerName()}/{innerAddress}/";
        }

        public static string FormatWithPayload(Type outerAddress, string innerAddress, object payload)
        {
            return $"{GetAddress(outerAddress, innerAddress)}{payload}";
        }
    }
}
