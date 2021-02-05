using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Injectors;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework
{
    public class AppBuilder<T> : IAppBuilder<T> where T : IBotContext
    {
        private readonly HashSet<Type>                       AlreadyAdded = new();
        private readonly IServiceCollection                  Services;
        private readonly Func<IServiceCollection, IInjector> builder;
        private readonly List<Type>                          midllewares    = new();
        private          Func<Update, T>                     contextCreator = null!;

        public AppBuilder(IServiceCollection services, Func<IServiceCollection, IInjector> builder)
        {
            Services     = services;
            this.builder = builder;
        }

        public void AddContextCreator(Func<Update, T> creator)
        {
            contextCreator = creator;
        }

        public void UseMiddleware<M>(Action<IServiceCollection> builder) where M : class, IMiddleware<T>
        {
            if (!AlreadyAdded.Contains(typeof(M)))
            {
                AlreadyAdded.Add(typeof(M));
                builder(Services);
            }

            midllewares.Add(typeof(M));
        }

        public IApp Build()
        {
            return new App<T>(builder(Services), midllewares, contextCreator);
        }
    }

    public class App<T> : IApp where T : IBotContext
    {
        private static readonly EndPoint        endPoint = new();
        private readonly        Func<Update, T> contextCreator;
        private readonly        IInjector       injector;
        private readonly        List<Type>      middlewares;

        public App(IInjector injector, List<Type> middlewares, Func<Update, T> contextCreator)
        {
            this.injector       = injector;
            this.middlewares    = middlewares;
            this.contextCreator = contextCreator;
        }

        public Task Run(Update update)
        {
            var scope   = injector.UseScope();
            var first   = (IMiddleware<T>) scope.Get(middlewares[0]);
            var current = first;
            for (var i = 1; i < middlewares.Count; i++)
            {
                var next = (IMiddleware<T>) scope.Get(middlewares[i]);
                
                current.Next = next;
                current      = next;
            }

            current.Next = endPoint;
            var context = contextCreator(update);
            return first.Invoke(context);
        }

        private class EndPoint : IMiddleware<T>
        {
            public IMiddleware<T> Next { get; set; } = null!;

            public Task Invoke(T context)
            {
                return Task.CompletedTask;
            }
        }
    }
}