using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework
{
    public interface IAppBuilder<T> where T:IBotContext
    {
        void AddContextCreator(Func<Update, T> creator);
        void UseScoped<M>() where M: class, IMiddleware<T>;
        void UseSingleton<M>() where M: class, IMiddleware<T>;
        IApp Build();
    }

    public interface IApp
    {
        Task Run(Update update);
    }
    public interface IBotContext
    {
       User    ChatId        { get; } 
       Update CurrentUpdate { get; }
    }

    public interface IDictionaryContext : IBotContext
    {
       /// <summary>
       /// First not done handler will handle CurrentUpdate
       /// </summary>
       LinkedList<IUpdateConsumer> Handlers { get; set; }
    }

    public interface IStaticCommandsContext : IDictionaryContext
    {
       List<Type> StaticCommands { get; }
    }

    public interface IUpdateConsumer
    {
        bool IsDone             { get; }
        bool IsWaitingForUpdate { get; }
        void Consume(Update update);
    }

    public interface IMiddleware<T> where T: IBotContext
    {
        IMiddleware<T> Next { get; set; }
        Task           Invoke(T context);
    }
}