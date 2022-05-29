using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Abstractions;
using BotFramework.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace BotFramework.Extensions.Hosting;

public static class ControllerDIExtensions
{
    public static void UseControllers(this IAppBuilder builder, params Type[] controllers)
    {
        foreach (var type in controllers)
        {
            builder.Services.TryAddScoped(type);
        }

        builder.Services.TryAddEnumerable(controllers.Select(c => ServiceDescriptor.Scoped(typeof(ICommandController), c)));
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IEndpoitBuilder, ControllerEndpointBuilder>(provider =>
            {
                provider.GetService<ILogger>()
                        ?.LogDebug("Loaded {Count} controllers: {Endpoints}",
                            controllers.Length,
                            string.Join(", ", controllers.Select(a => a.Name)));

                var collection = provider.GetService<IServiceCollection>()!
                                         .Where(a => a.ServiceType == typeof(ICommandController))
                                         .Select(a => a.ImplementationType)
                                         .Cast<Type>()
                                         .ToList();

                return new ControllerEndpointBuilder(collection);
            }
        ));
    }

    public static void UseControllers(this IAppBuilder builder)
    {
        var assemblies = StaticCommandsDIExtensions.GetAssemblies();
        builder.UseControllers(GetControllers(assemblies));
    }

    public static Type[] GetControllers(IEnumerable<Assembly> assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.GetTypes());
        var res = allTypes.Where(p => p.GetInterface("ICommandController") != null && !p.IsAbstract)
                          .ToArray();
        return res;
    }
}