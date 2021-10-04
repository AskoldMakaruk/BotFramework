using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;

namespace BotFramework.Middleware
{
    public record StaticCommandsList(IReadOnlyList<Type> StaticCommandsTypes);

    public class PossibleCommands
    {
        public List<ICommand> Commands { get; set; } = new();
    }

    public class StaticCommandsMiddleware
    {
        private readonly List<IStaticCommand> commands;
        private readonly UpdateDelegate       _next;

        public StaticCommandsMiddleware(IServiceProvider services, UpdateDelegate next, StaticCommandsList staticCommands)
        {
            _next = next;
            var scope = services.CreateScope();
            commands = staticCommands.StaticCommandsTypes.Select(scope.ServiceProvider.GetService)
                                     .Cast<IStaticCommand>()
                                     .ToList();
        }

        public Task Invoke(Update update, PossibleCommands possibleCommands)
        {
            possibleCommands.Commands.AddRange(commands);
            return _next.Invoke(update);
        }
    }

    public static class CommandsMiddlewareExtensions
    {
        public static void UsePossibleCommands(this IAppBuilder builder)
        {
            builder.Services.AddScoped(_ => new PossibleCommands());
        }

        public static void UseStaticCommands(this IAppBuilder builder, StaticCommandsList staticCommands)
        {
            builder.UsePossibleCommands();
            builder.Services.AddSingleton(staticCommands);
            builder.UseMiddleware<StaticCommandsMiddleware>(staticCommands);
            foreach (var command in staticCommands.StaticCommandsTypes)
                builder.Services.AddScoped(command);

            builder.Services.AddWrappedScoped(_ => new Consumers(new()));
            builder.UseMiddleware<StaticCommandsEndpoint>();
        }

        public static void UseStaticCommands(this IAppBuilder builder)
        {
            var staticCommands = GetStaticCommands();
            UseStaticCommands(builder, staticCommands);
        }

        public static StaticCommandsList GetStaticCommands()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                      .Where(a => !a.FullName.Contains("Microsoft")
                                                  && !a.FullName.Contains("System"))
                                      .ToList();

            var referenced = assemblies.SelectMany(a => a.GetReferencedAssemblies())
                                       .Where(a => !a.FullName.Contains("Microsoft")
                                                   && !a.FullName.Contains("System"))
                                       .Select(Assembly.Load);

            assemblies = assemblies.Concat(referenced).ToList();

            var allTypes = assemblies.SelectMany(a => a.GetTypes());
            var res = allTypes.Where(p => typeof(ICommand).IsAssignableFrom(p) && !p.IsAbstract)
                              .ToList();
            return new(res);
        }
    }
}