using System;
using BotFramework.Commands;

namespace BotFramework.Injectors
{
    public interface IInjectorBuilder
    {
        void      AddSingleton<T>() where T : class;
        void      AddScoped<T>() where T: class;
        IInjector Build();
    }
    public interface IInjector
    {
        public IInjectorScope UseScope();
        T      Get<T>();
        object Get(Type type);
    }

    public interface IInjectorScope
    {
        T      Get<T>();
        object Get(Type type);
    }
}