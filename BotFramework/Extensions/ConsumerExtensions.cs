using BotFramework.Abstractions;
using BotFramework.Clients;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BotFramework.Extensions
{
    public static class ConsumerExtensions
    {
        public static IServiceCollection AddUpdateConsumer(this IServiceCollection services)
        {
            services.AddScoped<IUpdateConsumer, Client>();
            services.AddTransient<UpdateQueue>();
            services.AddSingleton<IRequestSinc, TelegramSink>();

            return services;
        }

        public static IServiceCollection AddDebugUpdateConsumer(this IServiceCollection services)
        {
            services.AddScoped<IUpdateConsumer, Client>();
            services.AddTransient<UpdateQueue>();
            services.AddSingleton<AppUpdateProducer>();
            services.AddSingleton<IRequestSinc, MemorySink>();

            return services;
        }

        public static IServiceCollection AddTelegramClient(this IServiceCollection services, string token)
        {
            services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(token));
            return services;
        }
    }
}