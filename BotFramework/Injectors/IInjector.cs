using System;
using BotFramework.Commands;

namespace BotFramework.Injectors
{
    public interface IInjector
    {
        public ICommand Create(Type commandType);
        public T Create<T>() where T : ICommand;
    }
}