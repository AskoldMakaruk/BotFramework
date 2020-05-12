using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BotFramework.Bot;
using FastExpressionCompiler;
using Optional;
using Optional.Collections;
using Telegram.Bot.Types;
using static System.Linq.Expressions.Expression;

namespace BotFramework.Commands.Injectors
{
    public class CompilerInjector : DependencyInjector
    {
        //Dictionary <T, Validator<T>>
        public CompilerInjector(Dictionary<Type, Type> validators) => this.validators = validators;

        private readonly Dictionary<Type, Type> validators;

        private readonly Dictionary<Type, Func<Update, IGetOnlyClient, Option<ICommand>>> commandCreators =
            new Dictionary<Type, Func<Update, IGetOnlyClient, Option<ICommand>>>();

        public IEnumerable<ICommand> GetPossible(IEnumerable<Type> commandTypes, Update tgUpdate,
                                                 IGetOnlyClient    client)
        {
            return commandTypes.Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && !t.IsAbstract)
                               .Select(t =>
                               {
                                   if (!commandCreators.ContainsKey(t))
                                       commandCreators[t] = CompileCommand(t);
                                   return commandCreators[t](tgUpdate, client);
                               })
                               .Values()
                               .Where(t => t.Suitable);
        }

        public Func<Update, IGetOnlyClient, Option<ICommand>> CompileCommand(Type commandType)
        {
            var vars = new Dictionary<Type, ParameterExpression>
            {
                [typeof(IGetOnlyClient)] = Parameter(typeof(IGetOnlyClient), "client"),
                [typeof(Update)]         = Parameter(typeof(Update),         "update")
            };

            var body = CreateExpr(SortDependencies(commandType, new Stack<Type>(), new HashSet<Type>()), vars);
            var func = Lambda<Func<Update, IGetOnlyClient, Option<ICommand>>>(body: body,
                parameters: new[] {vars[typeof(Update)], vars[typeof(IGetOnlyClient)]});
            return func.CompileFast();
        }

        private Expression CreateExpr(Stack<Type> types, Dictionary<Type, ParameterExpression> variables)
        {
            var currentType = types.Pop();


            IEnumerable<ParameterExpression> GetParams(ConstructorInfo constructor) =>
            constructor.GetParameters()
                       .Select(t => t.ParameterType)
                       .Select(t =>
                       {
                           if (t == typeof(Update))
                               return variables[typeof(Update)];
                           if (t == typeof(IGetOnlyClient))
                               return variables[typeof(IGetOnlyClient)];
                           if (!variables.ContainsKey(t))
                               variables[t] = Parameter(t);
                           return variables[t];
                       });

            if (types.Count == 0)
            {
                var constructor = currentType.GetConstructors()[0];
                var asType      = TypeAs(New(constructor, GetParams(constructor)), typeof(ICommand));
                return Call(typeof(Option), "Some", new[] {typeof(ICommand)}, asType);
            }

            var validatorConstructor = validators[currentType].GetConstructors()[0];
            var callValidate = Call(New(validatorConstructor, GetParams(validatorConstructor)),
                validators[currentType].GetMethod("Validate")!);
            if (!variables.ContainsKey(currentType)) variables[currentType] = Parameter(currentType);
            var func = Lambda(CreateExpr(types, variables), variables[currentType]);
            var method = typeof(Option<>).MakeGenericType(currentType)
                                         .GetMethods()
                                         .Where(t => t.Name                         == "FlatMap")
                                         .Where(t => t.GetGenericArguments().Length == 1)
                                         .Select(t => t.MakeGenericMethod(func.ReturnType.GenericTypeArguments[0]))
                                         .First();
            var callFlatMap = Call(callValidate, method, func);
            return callFlatMap;
        }

        private Stack<Type> SortDependencies(Type currentType, Stack<Type> res, HashSet<Type> helper)
        {
            if (currentType == typeof(IGetOnlyClient) || currentType == typeof(Update)) return res;
            if (!helper.Contains(currentType))
            {
                res.Push(currentType);
                helper.Add(currentType);
            }

            foreach (var par in currentType.GetConstructors().Select(t => t.GetParameters()).First())
                SortDependencies(par.ParameterType, res, helper);
            return res;
        }
    }
}