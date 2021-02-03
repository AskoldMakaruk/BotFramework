using System;
using BotFramework.Injectors;

namespace BotFramework
{
    internal class WrappedInjector : IInjector
    {
        private readonly IInjector injector;
        public WrappedInjector(IInjector injector)
        {
            this.injector = injector;
        }

        public IInjectorScope UseScope() => new WrappedInjectorScope(injector.UseScope(), injector);

        public T      Get<T>()
        {
            if (typeof(T) == typeof(IInjector))
                return (T) injector;
            return injector.Get<T>();
        }

        public object Get(Type type)
        {
            if (type == typeof(IInjector))
                return injector;
            return injector.Get(type);
        }
    }
    internal class WrappedInjectorScope : IInjectorScope
    {
        private readonly IInjectorScope scope;
        private readonly IInjector      injector;
        public WrappedInjectorScope(IInjectorScope scope, IInjector injector)
        {
            this.scope    = scope;
            this.injector = injector;
        }

        public T      Get<T>()
        {
            if (typeof(T) == typeof(IInjector))
                return (T) injector;
            return scope.Get<T>();
        }

        public object Get(Type type)
        {
            if (type == typeof(IInjector))
                return injector;
            return scope.Get(type);
        }
    }
}