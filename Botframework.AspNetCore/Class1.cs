// using BotFramework.Abstractions;
// using Microsoft.AspNetCore.Http;
// using Microsoft.Extensions.Options;
// using Telegram.Bot.Types;
//
// namespace Botframework.AspNetCore
// {
//     public class BotFrameworkOptions
//     {
//         public string BotPath { get; set; }
//     }
//
//     // ReSharper disable once ClassNeverInstantiated.Global    
//     internal class BotFrameworkMiddleware
//     {
//         private readonly RequestDelegate     _next;
//         private readonly BotFrameworkOptions _options;
//
//         public BotFrameworkMiddleware(RequestDelegate next, IOptions<BotFrameworkOptions> options)
//         {
//             _next    = next ?? throw new ArgumentNullException(nameof(next));
//             _options = options.Value;
//         }
//
//         // ReSharper disable once UnusedMember.Global
//         public async Task Invoke(HttpContext httpContext, UpdateDelegate updateDelegate)
//         {
//             if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
//
//             if (!httpContext.Request.Path.Equals(new PathString(_options.BotPath)))
//             {
//                 await _next.Invoke(httpContext);
//                 return;
//             }
//
//             await updateDelegate.Invoke((await httpContext.Request.ReadFromJsonAsync<Update>())!);
//         }
//     }
// }