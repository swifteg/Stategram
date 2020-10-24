using Microsoft.VisualBasic.CompilerServices;
using Stategram;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TodoBot.Entities;

namespace TodoBot.Services
{
    class UIItemService
    {
        private readonly ITelegramBotClient _bot;

        public UIItemService(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task TickMessage(Message message)
        {
            await _bot.EditMessageReplyMarkupAsync(message.Chat, message.MessageId).ConfigureAwait(false);
            await _bot.EditMessageTextAsync(message.Chat, message.MessageId, "✅"+message.Text).ConfigureAwait(false);
            
        }

        public async Task RemoveItem(Message message)
        {
            await _bot.DeleteMessageAsync(message.Chat, message.MessageId).ConfigureAwait(false);
        }

        public static ResponseMessage FormatItem(TodoItem todo, string tickCallbackAddress, string removeCallbackAddress)
        {
            var tickButton = new InlineKeyboardButton()
            {
                Text = "✅",
                CallbackData = tickCallbackAddress
            };

            var removeButton = new InlineKeyboardButton()
            {
                Text = "❌",
                CallbackData = removeCallbackAddress
            };

            return new ResponseMessage()
            {
                Message = (todo.Finished ? "✅" : "") + todo.Text,
                InlineKeyboardButtons = !todo.Finished ? new InlineKeyboardButton[][] { new[] { tickButton, removeButton } }
                                                           : new InlineKeyboardButton[][] { new[] { removeButton } }
            };
        }
    }
}
