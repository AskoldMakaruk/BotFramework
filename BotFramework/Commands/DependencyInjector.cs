using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Bot;
using BotFramework.Commands.Validators;
using Optional;
using Optional.Collections;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public class DependencyInjector
    {
        //Dictionary <T, Validator<T>>
        public DependencyInjector(Dictionary<Type, Type> validators) => this.validators = validators;

        private Dictionary<Type, Type> validators { get; }

        public IEnumerable<ICommand> GetPossible(IEnumerable<Type> commandTypes, Update tgUpdate,
                                                 IGetOnlyClient    client)
        {
            return commandTypes.Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract)
                               .Select(t => Create(tgUpdate, client, t))
                               .Select(t =>
                               t.FlatMap(k => (k as ICommand).SomeNotNull()))
                               .Values();
        }

        private Option<object> Create(Update tgUpdate, IGetOnlyClient client, Type objType)
        {
            if (objType == typeof(Update)) return ((object) tgUpdate).Some();
            if (objType == typeof(IGetOnlyClient)) return ((object) client).Some();
            if (validators.ContainsKey(objType))
                return Create(tgUpdate, client, validators[objType])
                .FlatMap(validator => ((Validator) validator).Validate());

            if (objType.GetInterfaces().Contains(typeof(Validator)) || objType.GetInterfaces().Contains(typeof(ICommand)))
                foreach (var constructor in objType.GetConstructors())
                {
                    var paramTypes = constructor.GetParameters().Select(t => t.ParameterType).ToArray();
                    var parameters = paramTypes.Select(t => Create(tgUpdate, client, t)).Values().ToArray();
                    if (parameters.Length != paramTypes.Length)
                        continue;
                    return Activator.CreateInstance(objType, parameters).SomeNotNull();
                }

            return Option.None<object>();
        }

        //todo test
        public static void CopyAllParams(object newObject, object parentObject)
        {
            var type     = newObject.GetType();
            var baseType = parentObject.GetType();
            if (type.BaseType != baseType)
                throw new InvalidCastException($"{type} is not child of {baseType}");
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default;
            foreach (var field in baseType.GetFields(flags))
                field.SetValue(newObject, field.GetValue(parentObject));
            foreach (var property in baseType.GetProperties().Where(t => t.SetMethod != null && t.GetMethod != null))
               property.SetValue(newObject, property.GetValue(parentObject)); 

        }
    }
}