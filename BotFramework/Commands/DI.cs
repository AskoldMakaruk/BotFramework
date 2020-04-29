using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using BotFramework.Bot;
using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    internal class DI
    {
        //Dictionary <T, Validator<T>>
        public DI(Dictionary<Type, Type> validators) => this.validators = validators;

        private Dictionary<Type, Type> validators { get; }

        public List<ICommand> GetPossible(IEnumerable<Type> commandTypes, Update tgUpdate,
                                          IGetOnlyClient    client)
        {
            return commandTypes.Select(t => Create(tgUpdate, client, t))
                               .Select(t =>
                               t.SelectMany(k => new Optional<ICommand>(k as ICommand)))
                               .SelectJust()
                               .ToList();
        }

        private Optional<object> Create(Update tgUpdate, IGetOnlyClient client, Type objType)
        {
            if (objType == typeof(Update)) return ((object)tgUpdate).ToOptional();
            if (objType == typeof(IGetOnlyClient)) return ((object)client).ToOptional();
            if (validators.ContainsKey(objType))
            {
                var res = from validator in Create(tgUpdate, client, validators[objType])
                          from r in ((Validator) validator).Validate(tgUpdate, client)
                          select r;
                return res;
            }

            if (objType.GetInterfaces().Contains(typeof(Validator)) || objType.GetInterfaces().Contains(typeof(ICommand)))
            {
                foreach (var constructor in objType.GetConstructors())
                {
                    var paramTypes = constructor.GetParameters().Select(t => t.ParameterType).ToList();
                    var parameters = paramTypes.Select(t => Create(tgUpdate, client, t)).Sequence();
                    if (parameters.IsEmpty)
                        continue;
                    return Activator.CreateInstance(objType, parameters.ValueOr(null).ToArray()).ToOptional();
                }
            }

            return new Optional<object>();
        }
    }
}