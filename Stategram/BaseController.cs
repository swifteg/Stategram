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
        public int TelegramUserId { get; set; }
        public Message Message { get; set; }
        public CallbackQuery CallbackQuery { get; set; }
        public EventTypes Event { get; set; }
        public Dictionary<string, object> EventPayload { get; } = new Dictionary<string, object>();
        public long ChatId { get; set; }
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
