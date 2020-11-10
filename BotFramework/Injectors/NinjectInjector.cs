using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;

namespace BotFramework.Injectors
{
    public class NinjectInjector : IInjector
    {
        private IKernel kernel;

        public NinjectInjector(IReadOnlyList<Type> commands, IReadOnlyList<INinjectModule>? otherModules = null)
        {
            //assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract).ToList();
            var moduleList = new List<INinjectModule> {new CommandModule(commands)};
            if(otherModules != null)
                moduleList.AddRange(otherModules);
            kernel = new StandardKernel(moduleList.ToArray());
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