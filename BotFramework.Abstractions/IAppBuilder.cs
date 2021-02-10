using System;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework.Abstractions
{
    public interface IAppBuilder
    {
        IServiceCollection ApplicationServicesBuilder { get; set; }
        IAppBuilder    Use(Func<IServiceProvider, Func<UpdateDelegate, UpdateDelegate>> middleware);
        (IServiceProvider, UpdateDelegate) Build();
    }
}