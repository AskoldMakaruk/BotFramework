using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework
{
    public class AppBuilder : IAppBuilder
    {
        private readonly List<Func<IServiceProvider, Func<UpdateDelegate, UpdateDelegate>>> _components = new();


        /// <summary>
        /// Initializes a new instance of <see cref="IAppBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for application services.</param>
        public AppBuilder(IServiceCollection applicationServicesBuider)
        {
            ApplicationServicesBuilder = applicationServicesBuider;
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for application services.
        /// </summary>
        public IServiceCollection ApplicationServicesBuilder { get; set; }

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
        /// </summary>
        /// <returns>The <see cref="UpdateDelegate"/>.</returns>
        public (IServiceProvider, UpdateDelegate) Build()
        {
            var            provider = ApplicationServicesBuilder.BuildServiceProvider();
            UpdateDelegate app      = context => Task.CompletedTask;

            app = _components.Select(t => t(provider))
                             .Reverse()
                             .Aggregate(app, (current, component) => component(current));
            return (provider, app);
        }
    }
}