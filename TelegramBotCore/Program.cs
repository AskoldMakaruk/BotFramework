using System;
using TelegramBotCore.Telegram;
using TelegramBotCore.Telegram.Bot;

namespace TelegramBotCore
{
    internal class Program
    {
        abstract class One
        {
            public static string IsIt => "Yeah";
        }

        abstract class Twho : One
        {
            public new static string IsIt => "Nope";
        }

        private static void Main(string[] args)
        {
            Console.WriteLine(One.IsIt);
            Console.WriteLine(Twho.IsIt);


            var bot = new Client("{YOUR TOKEN HERE}");
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}