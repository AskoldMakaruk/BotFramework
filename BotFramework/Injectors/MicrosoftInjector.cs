using System;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Injectors
{
    public class MicrosoftInjectorBuilder : IInjectorBuilder
    {
        private readonly IServiceCollection services;

        public MicrosoftInjectorBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        public void AddSingleton<T>() where T : class
        {
            services.AddSingleton<T>();
        }


        public void AddScoped<T>() where T : class
        {
            services.AddScoped<T>();
        }

        public IInjector Build()
        {
            return new MicrosoftInjector(services.BuildServiceProvider());
        }
    }

    public class MicrosoftInjector : IInjector, IInjectorScope
    {
        private readonly IServiceProvider _provider;

        public MicrosoftInjector(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IInjectorScope UseScope()
        {
            return new MicrosoftInjector(_provider.CreateScope().ServiceProvider);
        }

        public T Get<T>()
        {
            var res = _provider.GetService<T>();
            if (res is null)
            {
                throw new Exception($"Cannot find service {nameof(T)}");
            }

            return res;
        }

        public object Get(Type type)
        {
            var res = _provider.GetService(type);
            if (res is null)
            {
                throw new Exception($"Cannot find service {type.Name}");
            }

            return res;
        }
    }
}