using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace BotFramework
{
    public class AppRunnerService : IHostedService
    {
        private readonly ITelegramBotClient _client;

        public AppRunnerService(UpdateDelegate app, ITelegramBotClient client)
        {
            _client          =  client;
            _client.OnUpdate += (sender, args) => app(args.Update);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.StartReceiving(cancellationToken: cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.StopReceiving();
            return Task.CompletedTask;
        }
    }

    public static class AppConfigurator
    {
        public static IHostBuilder ConfigureApp(this IHostBuilder builder, Action<IAppBuilder> appConfigurator)
        {
            return builder
                   .UseServiceProviderFactory(context => new AppBuilderFactory(context))
                   .ConfigureContainer<AppBuilder>((context, appBuilder) =>
                   {
                       appConfigurator(appBuilder);
                       appBuilder.Services.AddHostedService(provider =>
                       new AppRunnerService((UpdateDelegate) context.Properties["UniqueAppKostyl"],
                           provider.GetService<ITelegramBotClient>()!));
                   })
                   .UseConsoleLifetime();
        }
    }

    public class AppBuilderFactory : IServiceProviderFactory<AppBuilder>
    {
        private readonly HostBuilderContext _context;

        public AppBuilderFactory(HostBuilderContext context)
        {
            _context = context;
        }

        public AppBuilder CreateBuilder(IServiceCollection services)
        {
            return new(services);
        }

        public IServiceProvider CreateServiceProvider(AppBuilder containerBuilder)
        {
            var (services, app)                    = containerBuilder.Build();
            _context.Properties["UniqueAppKostyl"] = app;
            return services;
        }
    }
}