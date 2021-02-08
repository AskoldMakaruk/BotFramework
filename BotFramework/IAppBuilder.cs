using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework
{
    public delegate Task UpdateDelegate(Update update);

    public interface IAppBuilder
    {
        IServiceProvider ApplicationServices { get; set; }
        IAppBuilder    Use(Func<UpdateDelegate, UpdateDelegate> middleware);
        UpdateDelegate Build();
    }
}