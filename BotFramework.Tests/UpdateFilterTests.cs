using System;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Extensioins;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class UpdateFilterTests
    {
        private Mock<IClient> _clientMock;

        [SetUp]
        public void Setup()
        {
            _clientMock = new Mock<IClient>();

            var state = 1;
            Update GetUpdate() => new() { Id = state++ };
            _clientMock.Setup(client1 => client1.GetUpdate(It.IsAny<Func<Update, bool>>(), It.IsAny<Action<Update>>()))
                       .ReturnsAsync(GetUpdate);
        }

        [Test]
        public async Task UpdateFilter_WhenExcecuted_ShouldFilterMessages()
        {
            var client = _clientMock.Object;
            var result = await client.GetUpdateFilter();

            result.Id.Should().Be(1);
        }

        [Test]
        public async Task UpdateFilterWhere_WhenExcecuted_ShouldFilterMessages()
        {
            var client = _clientMock.Object;
            var result = await client.GetUpdateFilter().Where(update => update.Id == 1);

            result.Id.Should().Be(1);
        }

        [Test]
        public async Task WhereSelect_WhenExcecuted_ShouldFilterMessages()
        {
            var client = _clientMock.Object;
            var result = await client.GetUpdateFilter().Select(a => a.Id).Where(i => i == 1);

            result.Should().Be(1);
        }

        [Test]
        public async Task UpdateFilter_WhenExcecutedTwice_ShouldFilterMessages()
        {
            var client = _clientMock.Object;
            var filter = client.GetUpdateFilter().Select(a => a.Id).Where(i => i == 1);

            (await filter).Should().Be(1);
            (await filter).Should().Be(2);
        }
    }
}