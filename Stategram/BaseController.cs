using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Stategram
{
    public enum EventTypes
    {
        OnMessage,
        OnCallbackQuery
    }

    public class ControllerContext
    {
        public int TelegramUserId { get; init; }
        public Message Message { get; init; }
        public CallbackQuery CallbackQuery { get; init; }
        public EventTypes Event { get; init; }
        public Dictionary<string, object> EventPayload { get; } = new();
        public long ChatId { get; init; }
    }

    public abstract class BaseController
    {
        public delegate Task StatelessListenerType();

        public ControllerContext Context { get; set; }

        public string FormatCallback(StatelessListenerType method, object payload)
        {
            return Utils.Callback.FormatWithPayload(GetType(), method.Method.Name, payload);
        }
    }
}
