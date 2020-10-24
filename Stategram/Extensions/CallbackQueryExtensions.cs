using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace Stategram.Extensions
{
    public static class CallbackQueryExtensions
    {
        public const string StatelessCallbackRegex = "^([a-z]+)/([a-zA-Z]+)/(.*)$";
        
        public static bool IsStateless(this CallbackQuery callbackQuery)
        {
            var regex = new Regex(StatelessCallbackRegex);
            var match = regex.IsMatch(callbackQuery.Data);
            return match;
        }

        public static (string controllerName, string method) ExtractSerializedListener(this CallbackQuery callbackQuery)
        {
            var regex = new Regex(StatelessCallbackRegex);
            var matches = regex.Match(callbackQuery.Data);
            return (matches.Groups[1].Value, matches.Groups[2].Value);
        }

        public static string ExtractCallbackPayload(this CallbackQuery callbackQuery)
        {
            return string.Join("/", callbackQuery.Data.Split('/')[2..]);
        }

    }
}
