using System;
using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Execute();
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class StaticCommandAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class OnStartCommand : Attribute { }
}