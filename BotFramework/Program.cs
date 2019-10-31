using System;
using BotFramework.Client;

namespace TelegramBotCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            var bot = new Client("{YOUR TOKEN HERE}");
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}