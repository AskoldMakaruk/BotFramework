using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework
{
    public class WrappedServiceProvider
    {
        public WrappedServiceProvider(IServiceProvider provider)
        {
            Provider = provider;
        }
        public IServiceProvider Provider { get; set; }
    }
    public class AppBuilder : IAppBuilder
    {
        private readonly List<Func<IServiceProvider, Func<UpdateDelegate, UpdateDelegate>>> _components = new();


        /// <summary>
        /// Initializes a new instance of <see cref="IAppBuilder"/>.
        /// </summary>
        /// <param name="applicationServicesBuider">The <see cref="IServiceCollection"/> for application services.</param>
        public AppBuilder(IServiceCollection applicationServicesBuider)
        {
            Services = applicationServicesBuider;
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for application services.
        /// </summary>
        public IServiceCollection Services { get; set; }

        /// <summary>
        /// Adds the middleware to the application request pipeline.
        /// </summary>
        /// <param name="middleware">The middleware.</param>
        /// <returns>An instance of <see cref="IAppBuilder"/> after the operation has completed.</returns>
        public IAppBuilder Use(Func<IServiceProvider, Func<UpdateDelegate, UpdateDelegate>> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        /// <summary>
        /// Produces a <see cref="UpdateDelegate"/> that executes added middlewares.
        /// Also builds <see cref="IServiceProvider"/>
        /// </summary>
        /// <returns>The <see cref="IServiceProvider"/> and <see cref="UpdateDelegate"/>.</returns>
        public (IServiceProvider services, UpdateDelegate app) Build()
        {
            Services.AddScoped<WrappedServiceProvider>((serviceProvider => new(serviceProvider.CreateScope().ServiceProvider)));
            var            provider = Services.BuildServiceProvider();
            UpdateDelegate app      = context => Task.CompletedTask;

            app = _components.Select(t => t(provider))
                             .Reverse()
                             .Aggregate(app, (current, component) => component(current));

            return (provider, app);
        }
    }
}