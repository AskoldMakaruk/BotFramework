using System.Linq;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Middleware;
using BotFramework.Services.Commands;
using BotFramework.Services.Commands.Attributes;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace BotFramework.Tests.EndpointBuilders;

public class ControllerEndtoitBuilderTests
{
    private ControllerEndpointBuilder _builder;

    [SetUp]
    public void SetUp()
    {
        _builder = new ControllerEndpointBuilder(new[] { typeof(TestController1) });
    }


    [Test]
    public void Build_Should_Return_ControllerEndpoint()
    {
        var endpoint = _builder.Get().ToList();

        endpoint.Should().ContainSingle();
        endpoint.First()
                .Attributes.Select(a => a.GetType())
                .Should()
                .BeEquivalentTo(new[] { typeof(MockAttribute), typeof(CommandAttribute) });
    }

    [Mock]
    public class TestController1 : CommandControllerBase
    {
        public TestController1([NotNull] IClient client, [NotNull] UpdateContext update) : base(client, update) { }

        [Command("/start")]
        public async Task TestMethod1()
        {
            await Task.CompletedTask;
        }

        public void SomeMethod() { }
    }

    public class MockAttribute : CommandAttributeBase { }
}