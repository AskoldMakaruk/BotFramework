// using System.Collections.Generic;
// using System.Threading.Tasks;
// using BotFramework.Clients;
// using FluentAssertions;
// using NUnit.Framework;
// using Telegram.Bot.Types;
//
// namespace BotFramework.ControllersTests
// {
//     //todo implement tests
//     public class UpdateHandlerTests
//     {
//         private UpdateHandler _sink;
//
//
//         [SetUp]
//         public void Setup()
//         {
//             _sink = new UpdateHandler();
//         }
//
//
//         [Test]
//         public async Task should()
//         {
//             for (int i = 0; i < 10; i++)
//             {
//                 _sink.Consume(GetRequest(i));
//                 var message = await _sink.GetUpdate();
//                 message.Should().BeEquivalentTo(GetRequest(i));
//             }
//         }
//
//         [Test]
//         public async Task should2()
//         {
//             for (int i = 0; i < 10; i++)
//             {
//                 _sink.Consume(GetRequest(i));
//             }
//
//             for (int i = 0; i < 10; i++)
//             {
//                 var message = await _sink.GetUpdate();
//                 message.Should().BeEquivalentTo(GetRequest(i));
//             }
//         }
//
//         [Test]
//         [Ignore("This feature will not be implemented")]
//         public async Task should3()
//         {
//             var tasks = new List<Task>();
//             for (int i = 0; i < 10; i++)
//             {
//                 var i1        = i;
//                 var valueTask = _sink.GetUpdate();
//
//                 var task = valueTask
//                            .AsTask()
//                            .ContinueWith(a => a.GetAwaiter().GetResult().Should().BeEquivalentTo(GetRequest(i1)));
//                 tasks.Add(task);
//             }
//
//             for (int i = 0; i < 10; i++)
//             {
//                 _sink.Consume(GetRequest(i));
//             }
//
//             await Task.WhenAll(tasks.ToArray());
//         }
//
//
//         private Update GetRequest(int id)
//         {
//             return new Update
//             {
//                 Id = id,
//                 Message = new Message
//                 {
//                     Text = id.ToString()
//                 }
//             };
//         }
//     }
// }

