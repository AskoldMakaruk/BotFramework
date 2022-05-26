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
        _builder = new ControllerEndpointBuilder(new ControllersList(new[] { typeof(TestController1) }));
    }


    [Test]
    public void Build_Should_Return_ControllerEndpoint()
    {
        var endpoint = _builder.Get();
        endpoint.First()
                .Attributes.Select(a => a.GetType())
                .Should()
                .BeEquivalentTo(new[] { typeof(MockAttribute), typeof(StartsWithAttribute) });
    }

    [Mock]
    public class TestController1 : CommandControllerBase
    {
        public TestController1([NotNull] IClient client, [NotNull] UpdateContext update) : base(client, update) { }

        [StartsWith("/start")]
        public async Task TestMethod1()
        {
            await Task.CompletedTask;
        }
    }

    public class MockAttribute : CommandAttribute { }
}