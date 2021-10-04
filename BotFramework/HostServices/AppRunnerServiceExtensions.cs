using System;
using System.Xml;
using BotFramework.Abstractions;
using BotFramework.Clients;
using BotFramework.Middleware;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

namespace BotFramework.HostServices
{
    public static class AppRunnerServiceExtensions
    {
        public static IServiceCollection AddUpdateConsumer(this IServiceCollection services)
        {
            services.AddTransient<IUpdateConsumer, Client>();
            services.AddTransient<UpdateHandler>();

            return services;
        }

        public static IServiceCollection AddDebugUpdateConsumer(this IServiceCollection services)
        {
            services.AddSingleton<IUpdateConsumer, DebugClient>();
            services.AddTransient<UpdateHandler>();

            return services;
        }

        public static IServiceCollection AddTelegramClient(this IServiceCollection services, string token)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
            return services;
        }

        internal const string AppKostyl = "UniqueAppKostyl";

        public static IHostBuilder UseSimpleBotFramework(this IHostBuilder builder,
                                                         bool              isDebug = false)
        {
            return UseSimpleBotFramework(builder, (_, _) => { }, isDebug);
        }

        public static IHostBuilder UseSimpleBotFramework(this IHostBuilder                       builder,
                                                         Action<IAppBuilder, HostBuilderContext> appConfigurator,
                                                         bool                                    isDebug = false)
        {
            return builder.UseBotFramework((app, context) =>
            {
                appConfigurator?.Invoke(app, context);

                if (!isDebug)
                {
                    app.Services.AddUpdateConsumer();
                    app.Services.AddTelegramClient(context.Configuration["BotToken"]);
                }
                else
                {
                    app.Services.AddDebugUpdateConsumer();
                }

                app.UseMiddleware<LoggingMiddleware>();
                app.UseHandlers();
                app.UseStaticCommands();
            }, isDebug);
        }

        public static IHostBuilder UseBotFramework(this IHostBuilder                       builder,
                                                   Action<IAppBuilder, HostBuilderContext> appConfigurator,
                                                   bool                                    isDebug = false)
        {
            return builder
                   .UseServiceProviderFactory(context => new AppBuilderFactory(context))
                   .ConfigureContainer<AppBuilder>((context, appBuilder) =>
                   {
                       appConfigurator(appBuilder, context);


                       appBuilder.Services.AddHostedService(provider =>
                       new AppRunnerService((UpdateDelegate)context.Properties[AppKostyl],
                           provider.GetService<ITelegramBotClient>()!, provider.GetService<ILogger>()!));

                       if (isDebug)
                       {
                           appBuilder.Services.AddSingleton(_ =>
                           new DebugDelegateWrapper((UpdateDelegate)context.Properties[AppKostyl]));
                       }
                   })
                   .UseConsoleLifetime();
        }

        public record DebugDelegateWrapper(UpdateDelegate App);
    }
}