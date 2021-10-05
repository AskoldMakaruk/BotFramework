using System;
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
            services.AddSingleton<IRequestSinc, TelegramSink>();

            return services;
        }

        public static IServiceCollection AddDebugUpdateConsumer(this IServiceCollection services)
        {
            services.AddTransient<IUpdateConsumer, Client>();
            services.AddTransient<UpdateHandler>();
            services.AddSingleton<AppUpdateProducer>();
            services.AddSingleton<IRequestSinc, MemorySink>();

            return services;
        }

        public static IServiceCollection AddTelegramClient(this IServiceCollection services, string token)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
            return services;
        }

        internal const string AppKostyl = "UniqueAppKostyl";

        public static IHostBuilder UseSimpleBotFramework(this IHostBuilder builder, bool isTesting = false)
        {
            return UseSimpleBotFramework(builder, (_, _) => { }, isTesting);
        }

        public static IHostBuilder UseSimpleBotFramework(this IHostBuilder                       builder,
                                                         Action<IAppBuilder, HostBuilderContext> appConfigurator,
                                                         bool                                    isTesting = false)
        {
            return builder.UseBotFramework((app, context) =>
            {
                appConfigurator?.Invoke(app, context);

                if (!isTesting)
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
                app.UseMiddleware<SuitableMiddleware>();
            }, isTesting);
        }

        public static IHostBuilder UseBotFramework(this IHostBuilder                       builder,
                                                   Action<IAppBuilder, HostBuilderContext> appConfigurator,
                                                   bool                                    isTesting = false)
        {
            builder
            .UseServiceProviderFactory(context => new AppBuilderFactory(context))
            .ConfigureContainer<AppBuilder>((context, appBuilder) =>
            {
                appConfigurator(appBuilder, context);

                appBuilder.Services.AddHostedService(provider =>
                new AppRunnerService((UpdateDelegate)context.Properties[AppKostyl],
                    provider.GetService<ITelegramBotClient>()!, provider.GetService<ILogger>()!));

                if (isTesting)
                {
                    appBuilder.Services.AddSingleton(_ =>
                    new DebugDelegateWrapper((UpdateDelegate)context.Properties[AppKostyl]));
                }
            })
            .UseConsoleLifetime();

            return builder;
        }

        public static IHostBuilder UseBotFrameworkStartup<T>(this IHostBuilder builder, T startup) where T : IStartup =>
        builder.UseBotFramework(startup.Configure, startup.isTesting);

        public static IHostBuilder UseBotFrameworkStartup<T>(this IHostBuilder builder) where T : IStartup, new()
        {
            var startup = new T();
            return builder.UseBotFramework(startup.Configure, startup.isTesting);
        }

        public record DebugDelegateWrapper(UpdateDelegate App);
    }
}