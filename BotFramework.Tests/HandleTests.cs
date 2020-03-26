using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Responses;
using Moq;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class HandleTests
    {
        private Mock<Client> moq;

        [SetUp]
        public void Setup()
        {
            Console.SetOut(TestContext.Out);
            var _botConfiguration = new BotBuilder().WithToken(new string('0', 9) + ":" + new string('0', 35))
                                                    .UseConsoleLogger()
                                                    .WithStaticCommands(new SomeCommand())
                                                    .BuildConfiguration();
            moq = new Mock<Client>(_botConfiguration);
        }

        [Test]
        public async Task Handle()
        {
            moq.Setup(a => a.SendMessages(It.IsAny<IEnumerable<IResponseMessage>>()))
               .Returns((IEnumerable<IResponseMessage> messages) =>
               {
                   if (messages.First() is TextMessage message)
                   {
                       Assert.IsTrue(message.Text == "");
                   }
                   else
                   {
                       Assert.Fail();
                   }

                   return Task.CompletedTask;
               })
               .Callback(() => { Console.WriteLine("somesome"); });

            await moq.Object.HandleUpdate(new Update
            {
                Message = new Message
                {
                    Text = "true",
                    From = new User
                    {
                        Id = 0
                    }
                }
            });
        }

        [Test]
        public async Task Handle_2()
        {
            moq.Setup(a => a.SendMessages(It.IsAny<IEnumerable<IResponseMessage>>()))
               .Returns((IEnumerable<IResponseMessage> messages) =>
               {
                   Assert.Fail();
                   return Task.CompletedTask;
               });

            await moq.Object.HandleUpdate(new Update
            {
                Message = new Message
                {
                    Text = "false",
                    From = new User
                    {
                        Id = 0
                    }
                }
            });
        }

        [Test]
        public async Task Handle_null()
        {
            moq.Setup(a => a.SendMessages(It.IsAny<IEnumerable<IResponseMessage>>()))
               .Returns((IEnumerable<IResponseMessage> messages) =>
               {
                   Assert.Fail();
                   return Task.CompletedTask;
               });

            await moq.Object.HandleUpdate((Update) null);
        }

        [Test]
        public async Task Handle_withInline()
        {
            var text = "some text to test";
            var _botConfiguration = new BotBuilder().WithToken(new string('0', 9) + ":" + new string('0', 35))
                                                    .UseConsoleLogger()
                                                    .WithStaticCommands(CommandFactory.CreateCommand(
                                                        (update, client) =>
                                                        new Response().AddMessage(new TextMessage(0, update.Message.Text)),
                                                        update => true))
                                                    .BuildConfiguration();
            moq = new Mock<Client>(_botConfiguration);
            moq.Setup(a => a.SendMessages(It.IsAny<IEnumerable<IResponseMessage>>()))
               .Returns((IEnumerable<IResponseMessage> messages) =>
               {
                   if (messages.First() is TextMessage message)
                   {
                       Assert.AreEqual(message.Text, text);
                   }
                   else
                   {
                       Assert.Fail();
                   }

                   return Task.CompletedTask;
               });
            await moq.Object.HandleUpdate(new Update
            {
                Message = new Message
                {
                    Text = text,
                    From = new User
                    {
                        Id = 0
                    }
                }
            });
        }
    }
}