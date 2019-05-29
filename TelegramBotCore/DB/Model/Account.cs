namespace TelegramBotCore.DB.Model
{
    public class Account
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public long ChatId { get; set; }

        public AccountStatus Status { get; set; }
    }

    public enum AccountStatus
    {
        Free,
        Start,
        Balance,
        SelectAccount
    }
}