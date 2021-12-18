using System;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Abstractions
{
    public interface IAppBuilder
    {
        IServiceCollection Services { get; set; }
        IAppBuilder Use(Func<IServiceProvider, Func<UpdateDelegate, UpdateDelegate>> middleware);
        (IServiceProvider services, UpdateDelegate app) Build();
    }
}