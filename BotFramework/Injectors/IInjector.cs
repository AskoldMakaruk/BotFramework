using System;

namespace BotFramework.Injectors
{
    public interface IInjector
    {
        public IInjectorScope UseScope();
        T                     Get<T>();
        object                Get(Type type);
    }

    public interface IInjectorScope
    {
        T      Get<T>();
        object Get(Type type);
    }
}