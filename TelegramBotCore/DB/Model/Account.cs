namespace TelegramBotCore.DB.Model
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public long ChatId { get; set; }
        public string Language { get; set; }

        public AccountStatus Status { get; set; }
    }

    public enum AccountStatus
    {
        Free,
        Start,
    }
}