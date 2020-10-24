using System;
using System.Collections.Generic;
using System.Text;

namespace TodoBot.Entities
{
    class TodoItem
    {
        public int Id { get; set; }
        public int TelegramUserId { get; set; }
        public string Text { get; set; }
        public bool Finished { get; set; }
    }
}
