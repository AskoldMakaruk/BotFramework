using System;
using System.Collections.Generic;

namespace BotFramework.Middleware;

public record ControllersList(IReadOnlyList<Type> Types);