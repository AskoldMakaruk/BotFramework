using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject.Infrastructure.Language;

namespace BotFramework
{
    public class AppBuilder : IAppBuilder
    {
        private readonly List<Func<UpdateDelegate, UpdateDelegate>> _components = new();

        /// <summary>
        /// Initializes a new instance of <see cref="IAppBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> for application services.</param>
        public AppBuilder(IServiceProvider serviceProvider)
        {
            ApplicationServices = serviceProvider;
        }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> for application services.
        /// </summary>
        public IServiceProvider ApplicationServices { get; set; }

        /// <summary>
        /// Adds the middleware to the application request pipeline.
        /// </summary>
        /// <param name="middleware">The middleware.</param>
        /// <returns>An instance of <see cref="IAppBuilder"/> after the operation has completed.</returns>
        public IAppBuilder Use(Func<UpdateDelegate, UpdateDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }

        /// <summary>
        /// Produces a <see cref="UpdateDelegate"/> that executes added middlewares.
        /// </summary>
        /// <returns>The <see cref="UpdateDelegate"/>.</returns>
        public UpdateDelegate Build()
        {
            UpdateDelegate app = context => Task.CompletedTask;

            app = _components.ToEnumerable()
                             .Reverse()
                             .Aggregate(app, (current, component) => component(current));
            return app;
        }
    }
}