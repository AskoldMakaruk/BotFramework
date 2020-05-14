using System;
using BotFramework.Responses;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Execute();
        bool Suitable => true;
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class StaticCommandAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class OnStartCommand : Attribute { }
}