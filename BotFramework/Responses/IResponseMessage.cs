using System.Threading.Tasks;
using Telegram.Bot;

namespace BotFramework.Responses
{
    public interface IResponseMessage
    {
        Task Send(TelegramBotClient botClient);
    }
}