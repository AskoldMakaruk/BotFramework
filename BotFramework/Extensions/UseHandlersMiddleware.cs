// using BotFramework.Abstractions;
// using BotFramework.Middleware;
// using Microsoft.Extensions.DependencyInjection;
//
// namespace BotFramework.Extensions
// {
//     public static class UseHandlersMiddleware
//     {
//         public static void UseHandlers(this IAppBuilder builder)
//         {
//             builder.Services.AddSingleton<ContextDictionary>();
//             builder.UseMiddleware<DictionaryCreatorMiddleware>();
//         }
//     }
// }