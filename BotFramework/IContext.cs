using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework
{
    public interface IBotContext
    {
        User   ChatId        { get; }
        Update CurrentUpdate { get; }
    }

    public interface IUpdateConsumer
    {
        bool IsDone             { get; }
        bool IsWaitingForUpdate { get; }
        void Consume(Update update);
    }

    public interface IMiddleware
    {
        IMiddleware Next { get; set; }
        Task           Invoke(Update update, UpdateDelegate next);
    }
}