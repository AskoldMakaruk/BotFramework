using System.ComponentModel.DataAnnotations;

namespace TelegramBotCore.DB.Model
{
    public class Text
    {
        [Key]
        public string Key { get; set; }

        [Key]
        public string Language { get; set; }
        public string Value { get; set; }

    }
}