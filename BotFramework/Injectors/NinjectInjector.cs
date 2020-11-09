using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using Ninject;

namespace BotFramework.Injectors
{
    public class NinjectInjector : IInjector
    {
        private IKernel kernel;

        public NinjectInjector(IReadOnlyList<Type> commands)
        {
            //assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract).ToList();
            kernel = new StandardKernel(new CommandModule(commands));
        }
        public ICommand Create(Type commandType)
        {
            return (ICommand) kernel.Get(commandType);
        }

        public T Create<T>() where T : ICommand
        {
            return kernel.Get<T>();
        }

        class CommandModule : Ninject.Modules.NinjectModule
        {
            private readonly IReadOnlyList<Type> commands;

            public override void Load()
            {
                foreach (var command in commands)
                   Bind(command).ToSelf();
            }

            public CommandModule(IReadOnlyList<Type> Commands)
            {
                commands = Commands;
            }
        }
    }
}