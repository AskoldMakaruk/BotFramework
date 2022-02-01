using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

public interface IAppBuilder
{
    IServiceCollection Services { get; set; }

    IAppBuilder Use(Func<IServiceProvider, Func<UpdateDelegate, UpdateDelegate>> middleware);

    (IServiceProvider services, BotDelegate app) Build();
}

public delegate Task BotDelegate(Update update);