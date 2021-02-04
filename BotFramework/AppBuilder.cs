using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Injectors;
using Telegram.Bot.Types;

namespace BotFramework
{
    public class AppBuilder<T> : IAppBuilder<T> where T : IBotContext
    {
        private readonly HashSet<Type>    AlreadyAdded = new();
        private readonly IInjectorBuilder InjectorBuilder;
        private readonly List<Type>       midllewares    = new();
        private          Func<Update, T>  contextCreator = null!;

        public AppBuilder(IInjectorBuilder injectorBuilder)
        {
            InjectorBuilder = injectorBuilder;
        }

        public void AddContextCreator(Func<Update, T> creator)
        {
            contextCreator = creator;
        }

        public void UseScoped<M>() where M : class, IMiddleware<T>
        {
            if (!AlreadyAdded.Contains(typeof(M)))
            {
                AlreadyAdded.Add(typeof(M));
                InjectorBuilder.AddScoped<M>();
            }

            midllewares.Add(typeof(M));
        }

        public void UseSingleton<M>() where M : class, IMiddleware<T>
        {
            if (!AlreadyAdded.Contains(typeof(M)))
            {
                AlreadyAdded.Add(typeof(M));
                InjectorBuilder.AddSingleton<M>();
            }

            midllewares.Add(typeof(M));
        }

        public IApp Build()
        {
            return new App<T>(new WrappedInjector(InjectorBuilder.Build()), midllewares, contextCreator);
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