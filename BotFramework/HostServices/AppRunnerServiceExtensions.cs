using System;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace BotFramework.HostServices
{
    public static class AppRunnerServiceExtensions
    {
        internal const string AppKostyl = "UniqueAppKostyl";

        public static IHostBuilder ConfigureApp(this IHostBuilder builder, Action<IAppBuilder> appConfigurator)
        {
            return builder
                   .UseServiceProviderFactory(context => new AppBuilderFactory(context))
                   .ConfigureContainer<AppBuilder>((context, appBuilder) =>
                   {
                       appConfigurator(appBuilder);
                       appBuilder.Services.AddHostedService(provider =>
                       new AppRunnerService((UpdateDelegate) context.Properties[AppKostyl],
                           provider.GetService<ITelegramBotClient>()!));
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
                              appBuilder.Services.AddSingleton(provider =>
                              new DebugDelegateWrapper((UpdateDelegate) context.Properties[AppKostyl]));
                          });
        }
    }
}