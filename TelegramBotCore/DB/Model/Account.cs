using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types;
using TelegramBotCore.Controllers;
using TelegramBotCore.Telegram;
using TelegramBotCore.Telegram.Commands;

namespace TelegramBotCore.DB.Model
{
    public class Account
    {
        public int    Id       { get; set; }
        public long   ChatId   { get; set; }
        public string Name     { get; set; }
        public string Language { get; set; }

        public CommandStatus Status { get; set; }

        [NotMapped] public TelegramController Controller { get; set; }

        public static implicit operator ChatId(Account a) => a.ChatId;
    }
}