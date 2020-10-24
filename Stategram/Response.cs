using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace Stategram
{
    public class ResponseMessage
    {
        public string Message { get; set; }

        public IEnumerable<string> Images { get; set; }

        public IEnumerable<IEnumerable<string>> ReplyKerboardButtons { get; set; }

        public IEnumerable<IEnumerable<InlineKeyboardButton>> InlineKeyboardButtons { get; set; }

        public int ReplyToId { get; set; } = 0;

        public ResponseMessage ReplyToResponseMessage { get; set; } = null;
    }

    public class Response
    {
        public Response(IEnumerable<ResponseMessage> responseMessages)
        {
            ResponseMessages = responseMessages;
        }

        public IEnumerable<ResponseMessage> ResponseMessages { get; }


    }
}
