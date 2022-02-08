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
    public static void UseControllers(this IAppBuilder builder, ControllersList controllers)
    {
        builder.Services.TryAddSingleton(provider =>
        {
            provider.GetService<ILogger>()
                    ?.LogDebug("Loaded {Count} controllers: {Endpoints}",
                        controllers.Types.Count,
                        string.Join(", ", controllers.Types.Select(a => a.Name)));

            return controllers;
        });

        foreach (var controller in controllers.Types)
        {
            builder.Services.AddScoped(controller);
        }
    }

    public static void UseControllers(this IAppBuilder builder)
    {
        if (builder.Services.Any(x => x.ServiceType == typeof(ControllersList)))
        {
            return;
        }

        var assemblies = StaticCommandsDIExtensions.GetAssemblies();


        builder.UseControllers(GetControllers(assemblies));
    }

    public static ControllersList GetControllers(IEnumerable<Assembly> assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.GetTypes());
        var res = allTypes.Where(p => p.GetInterface("ICommandController") != null && !p.IsAbstract)
                          .ToList();
        return new(res);
    }
}