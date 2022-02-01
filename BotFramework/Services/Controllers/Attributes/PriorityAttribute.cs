using BotFramework.Abstractions;

namespace BotFramework.Services.Controllers.Attributes;

public class PriorityAttribute : CommandAttribute
{
    public override EndpointPriority? EndpointPriority { get; }

    public PriorityAttribute(EndpointPriority priority)
    {
        EndpointPriority = priority;
    }

    public override bool? Suitable(UpdateContext context)
    {
        return true;
    }
}