using System.ComponentModel.DataAnnotations;

namespace TelegramBotCore.DB.Model
{
    public class DbButton
    {
        public string Value { get; set; }
        [Key]
        public string Key { get; set; }
    }
}