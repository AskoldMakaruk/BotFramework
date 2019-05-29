using System.ComponentModel.DataAnnotations;

namespace TelegramBotCore.DB.Model
{
    public class DbMessage
    {
        public string Value { get; set; }
        [Key]
        public string Key { get; set; }
    }
}