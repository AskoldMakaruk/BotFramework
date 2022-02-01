using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Hosting;

public static class UseMiddlewareExtensions
{
    internal const string InvokeMethodName      = "Invoke";
    internal const string InvokeAsyncMethodName = "InvokeAsync";

    private static readonly MethodInfo GetServiceInfo =
    typeof(UseMiddlewareExtensions).GetMethod(nameof(GetService), BindingFlags.NonPublic | BindingFlags.Static)!;

    // We're going to keep all public constructors and public methods on middleware
    private const DynamicallyAccessedMemberTypes MiddlewareAccessibility =
    DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicMethods;

    /// <summary>
    /// Adds a middleware type to the application's request pipeline.
    /// </summary>
    /// <typeparam name="TMiddleware">The middleware type.</typeparam>
    /// <param name="app">The <see cref="IAppBuilder"/> instance.</param>
    /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
    /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
    public static IAppBuilder UseMiddleware<[DynamicallyAccessedMembers(MiddlewareAccessibility)] TMiddleware>(
        this IAppBuilder app, params object[] args)
    {
        return app.UseMiddleware(typeof(TMiddleware), args);
    }

    /// <summary>
    /// Adds a middleware type to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IAppBuilder"/> instance.</param>
    /// <param name="middleware">The middleware type.</param>
    /// <param name="args">The arguments to pass to the middleware type instance's constructor.</param>
    /// <returns>The <see cref="IAppBuilder"/> instance.</returns>
    public static IAppBuilder UseMiddleware(this IAppBuilder app,
                                            [DynamicallyAccessedMembers(MiddlewareAccessibility)]
                                            Type middleware,
                                            params object[] args)
    {
        Func<UpdateDelegate, UpdateDelegate> Middleware(IServiceProvider applicationServices) => next =>
        {
            var methodInfo = GetMiddlewareInvokeMethod(middleware);

            var parameters = GetMiddlewareInvokeParameters(methodInfo);

            var instance = GetMiddlewareInstance(middleware, args, next, applicationServices);

            if (parameters.Length == 1)
            {
                return (UpdateDelegate)methodInfo.CreateDelegate(typeof(UpdateDelegate), instance);
            }

            var factory = Compile<object>(methodInfo, parameters);

            return update =>
            {
                if (applicationServices == null)
                {
                    throw new InvalidOperationException(
                        Resources.FormatException_UseMiddlewareIServiceProviderNotAvailable(nameof(IServiceProvider)));
                }

                var result = factory(instance, update, applicationServices);
                if (result.Exception != null)
                {
                    throw result.Exception;
                }

                return result;
            };
        };

        return app.Use(Middleware);
    }

    private static object GetMiddlewareInstance(Type             middleware, object[] args, UpdateDelegate next,
                                                IServiceProvider applicationServices)
    {
        var ctorArgs = new object[args.Length + 1];
        ctorArgs[0] = next;
        Array.Copy(args, 0, ctorArgs, 1, args.Length);
        var instance = ActivatorUtilities.CreateInstance(applicationServices, middleware, ctorArgs);
        return instance;
    }

    private static ParameterInfo[] GetMiddlewareInvokeParameters(MethodBase methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        if (parameters.Length == 0 || parameters[0].ParameterType != typeof(UpdateContext))
        {
            throw new InvalidOperationException(
                Resources.FormatException_UseMiddlewareNoParameters(InvokeMethodName, InvokeAsyncMethodName,
                    nameof(Update)));
        }

        return parameters;
    }

    private static MethodInfo GetMiddlewareInvokeMethod(IReflect middleware)
    {
        var methods = middleware.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                                .Concat(middleware.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic));
        var invokeMethods = methods.Where(m => string.Equals(m.Name,    InvokeMethodName,      StringComparison.Ordinal)
                                               || string.Equals(m.Name, InvokeAsyncMethodName, StringComparison.Ordinal))
                                   .ToArray();

        if (invokeMethods.Length > 1)
        {
            throw new InvalidOperationException(
                Resources.FormatException_UseMiddleMutlipleInvokes(InvokeMethodName, InvokeAsyncMethodName));
        }

        if (invokeMethods.Length == 0)
        {
            throw new InvalidOperationException(
                Resources.FormatException_UseMiddlewareNoInvokeMethod(InvokeMethodName, InvokeAsyncMethodName,
                    middleware));
        }

        var methodInfo = invokeMethods[0];
        if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
        {
            throw new InvalidOperationException(
                Resources.FormatException_UseMiddlewareNonTaskReturnType(InvokeMethodName, InvokeAsyncMethodName,
                    nameof(Task)));
        }

        return methodInfo;
    }

    private static Func<T, UpdateContext, IServiceProvider, Task> Compile<T>(MethodInfo methodInfo, ParameterInfo[] parameters)
    {
        // If we call something like
        //
        // public class Middleware
        // {
        //    public Task Invoke(UpdateContext context, ILoggerFactory loggerFactory)
        //    {
        //
        //    }
        // }
        //

        // We'll end up with something like this:
        //   Generic version:
        //
        //   Task Invoke(Middleware instance, UpdateContext UpdateContext, IServiceProvider provider)
        //   {
        //      return instance.Invoke(UpdateContext, (ILoggerFactory)UseMiddlewareExtensions.GetService(provider, typeof(ILoggerFactory));
        //   }

        //   Non generic version:
        //
        //   Task Invoke(object instance, UpdateContext UpdateContext, IServiceProvider provider)
        //   {
        //      return ((Middleware)instance).Invoke(UpdateContext, (ILoggerFactory)UseMiddlewareExtensions.GetService(provider, typeof(ILoggerFactory));
        //   }

        var middleware = typeof(T);

        var updateArg   = Expression.Parameter(typeof(UpdateContext),           "update");
        var providerArg = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");
        var instanceArg = Expression.Parameter(middleware,               "middleware");

        var methodArguments = new Expression[parameters.Length];
        methodArguments[0] = updateArg;
        for (int i = 1; i < parameters.Length; i++)
        {
            var parameterType = parameters[i].ParameterType;
            if (parameterType.IsByRef)
            {
                throw new NotSupportedException(
                    Resources.FormatException_InvokeDoesNotSupportRefOrOutParams(InvokeMethodName));
            }

            var parameterTypeExpression = new Expression[]
            {
                providerArg,
                Expression.Constant(parameterType,            typeof(Type)),
                Expression.Constant(methodInfo.DeclaringType, typeof(Type))
            };

            var getServiceCall = Expression.Call(GetServiceInfo, parameterTypeExpression);
            methodArguments[i] = Expression.Convert(getServiceCall, parameterType);
        }

        Expression middlewareInstanceArg = instanceArg;
        if (methodInfo.DeclaringType != null && methodInfo.DeclaringType != typeof(T))
        {
            middlewareInstanceArg = Expression.Convert(middlewareInstanceArg, methodInfo.DeclaringType);
        }

        var body = Expression.Call(middlewareInstanceArg, methodInfo, methodArguments);

        var lambda = Expression.Lambda<Func<T, UpdateContext, IServiceProvider, Task>>(body, instanceArg, updateArg,
            providerArg);

        return lambda.Compile();
    }

    private static object GetService(IServiceProvider sp, Type type, Type middleware)
    {
        var service = sp.GetService(type);
        if (service == null)
        {
            throw new InvalidOperationException(Resources.FormatException_InvokeMiddlewareNoService(type, middleware));
        }

        return service;
    }
}