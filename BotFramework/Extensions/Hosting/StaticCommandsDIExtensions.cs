using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Hosting;
using BotFramework.Middleware;
using BotFramework.Services.Commands.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace BotFramework.Extensions.Hosting;

public static class StaticCommandsDIExtensions
{
    public static void UseStaticCommands(this IAppBuilder builder, List<Type> commands)
    {
        foreach (var type in commands)
        {
            builder.Services.TryAddScoped(type);
        }

        builder.Services.TryAddEnumerable(commands.Select(c => ServiceDescriptor.Scoped(typeof(ICommand), c)));
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IEndpoitBuilder, CommandEndpointBuilder>(provider =>
            {
                provider.GetService<ILogger>()
                        ?.LogDebug("Loaded {Count} static commands: {Endpoints}",
                            commands.Count,
                            string.Join(", ", commands.Select(a => a.Name)));

                var collection = provider.GetService<IServiceCollection>()!
                                         .Where(a => a.ServiceType == typeof(ICommand))
                                         .Select(a => a.ImplementationType)
                                         .Cast<Type>()
                                         .ToList();

                return new CommandEndpointBuilder(provider, collection);
            }
        ));

        builder.UseMiddleware<CommandEndpointMiddleware>();
    }

    public static void UseStaticCommandsAssembly(this IAppBuilder builder, Assembly assembly)
    {
        builder.UseStaticCommands(GetStaticCommands(new[] { assembly }));
    }

    public static void UseStaticCommands(this IAppBuilder builder)
    {
        builder.UseStaticCommands(GetStaticCommands(GetAssemblies()));
    }

    // this might not be perfect solution because it loads many assemblies 
    // we need to investigate it later
    internal static Assembly[] GetAssemblies()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                  .Where(a => !a.FullName.Contains("Microsoft")
                                              && !a.FullName.Contains("System"))
                                  .ToList();

        var referenced = assemblies.SelectMany(a => a.GetReferencedAssemblies())
                                   .Where(a => !a.FullName.Contains("Microsoft")
                                               && !a.FullName.Contains("System"))
                                   .Select(Assembly.Load);


        return assemblies.Concat(referenced).ToArray();
    }

    public static List<Type> GetStaticCommands(IEnumerable<Assembly> assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.GetTypes());
        var res = allTypes.Where(p => typeof(ICommand).IsAssignableFrom(p)
                                      && !p.IsAbstract
                                      && p.GetCustomAttributes(true)
                                          .All(a => a.GetType() != typeof(IgnoreReflectionAttribute)))
                          .ToList();
        return res;
    }
}