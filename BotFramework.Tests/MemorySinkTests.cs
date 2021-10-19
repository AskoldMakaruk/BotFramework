using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Services.Clients;
using FluentAssertions;
using NUnit.Framework;
using Serilog;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class MemorySinkTests
    {
        private MemorySink _sink;


        [SetUp]
        public void Setup()
        {
            var logger = new LoggerConfiguration().MinimumLevel.Verbose()
                                                  .WriteTo.Console()
                                                  .CreateLogger();

            _sink = new MemorySink(logger);
        }


        [Test]
        public async Task should()
        {
            for (int i = 0; i < 10; i++)
            {
                await _sink.MakeRequest(GetRequest(i));
                var message = await _sink.GetRequest<SendMessageRequest>();
                message.ChatId.Should().Be(i);
            }
        }

        [Test]
        public async Task should2()
        {
            for (int i = 0; i < 10; i++)
            {
                await _sink.MakeRequest(GetRequest(i));
            }

            for (int i = 0; i < 10; i++)
            {
                var message = await _sink.GetRequest<SendMessageRequest>();
                message.ChatId.Should().Be(i);
            }
        }

        [Test]
        public async Task should3()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 10; i++)
            {
                var i1        = i;
                var valueTask = _sink.GetRequest<SendMessageRequest>();

                var task = valueTask
                           .AsTask()
                           .ContinueWith(a => a.GetAwaiter().GetResult().ChatId.Should().Be(i1));
                tasks.Add(task);
            }

            for (int i = 0; i < 10; i++)
            {
                await _sink.MakeRequest(GetRequest(i));
            }

            await Task.WhenAll(tasks.ToArray());
        }


        private IRequest<Message> GetRequest(int id)
        {
            return new SendMessageRequest(id, id.ToString());
        }
    }
}