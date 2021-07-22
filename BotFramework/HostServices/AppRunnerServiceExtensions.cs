using System;
using System.Xml;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Telegram.Bot;

namespace BotFramework.HostServices
{
    public interface IStartup
    {
        void ConfigureServices(IServiceCollection services);

        void Configure(IAppBuilder app);
    }

    public static class AppRunnerServiceExtensions
    {
        internal const string AppKostyl = "UniqueAppKostyl";

        public static IHostBuilder UseStartup<T>(this IHostBuilder builder) where T : class, IStartup
        {
            
            builder.ConfigureServices(collection =>
            {
                collection.AddSingleton<T>();
            });

            return builder;
        }

        public static IHostBuilder ConfigureApp(this IHostBuilder                       builder,
                                                Action<IAppBuilder, HostBuilderContext> appConfigurator)
        {
            return builder
                   .UseServiceProviderFactory(context => new AppBuilderFactory(context))
                   .ConfigureContainer<AppBuilder>((context, appBuilder) =>
                   {
                       appConfigurator(appBuilder, context);
                       appBuilder.Services.AddHostedService(provider =>
                       new AppRunnerService((UpdateDelegate)context.Properties[AppKostyl],
                           provider.GetService<ITelegramBotClient>()!, provider.GetService<ILogger>()!));
                   })
                   .UseConsoleLifetime();
        }

        public record DebugDelegateWrapper(UpdateDelegate App);

        public static IHostBuilder ConfigureAppDebug(this IHostBuilder builder, Action<IAppBuilder> appConfigurator)
        {
            return builder.UseServiceProviderFactory(context => new AppBuilderFactory(context))
                          .ConfigureContainer<AppBuilder>((context, appBuilder) =>
                          {
                              appConfigurator(appBuilder);
                              appBuilder.Services.AddSingleton(_ =>
                              new DebugDelegateWrapper((UpdateDelegate)context.Properties[AppKostyl]));
                          });
        }
    }
}