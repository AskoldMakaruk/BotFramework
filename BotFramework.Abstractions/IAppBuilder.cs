using System;

namespace BotFramework.Abstractions
{
    public interface IAppBuilder
    {
        IServiceProvider ApplicationServices { get; set; }
        IAppBuilder    Use(Func<UpdateDelegate, UpdateDelegate> middleware);
        UpdateDelegate Build();
    }
}