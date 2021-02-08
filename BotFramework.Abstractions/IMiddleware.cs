using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions
{
    public interface IMiddleware
    {
        IMiddleware Next { get; set; }
        Task        Invoke(Update update, UpdateDelegate next);
    }
}