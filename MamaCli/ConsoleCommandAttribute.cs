using System;

namespace MamaCli
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class ConsoleCommandAttribute : Attribute
    {
        public ConsoleKey Key      { get; set; }
        public string     HelpText { get; set; }

        public ConsoleCommandAttribute(ConsoleKey trigerKey, string helpText = null)
        {
            Key      = trigerKey;
            HelpText = helpText;
        }
    }
}