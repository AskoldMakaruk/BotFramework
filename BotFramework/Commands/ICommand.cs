using System;
using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Execute(Update message, IGetOnlyClient client);

        bool Suitable(Update message);
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class StaticCommandAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class OnStartCommand : Attribute { }

    [Obsolete("Use [StaticCommand] instead")]
    [StaticCommand]
    public interface IStaticCommand : ICommand { }
}