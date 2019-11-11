using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace MamaCli
{
    internal class CliStart
    {
        public static  NamedPipeClientStream client;
        private static string                Message;

        static void Main(string[] args)
        {
            Message = string.Join(' ', args) + '\n';
            client  = new NamedPipeClientStream(".", "BotMamaPipe", PipeDirection.InOut, PipeOptions.Asynchronous);
            client.Connect();

            var buffer = Encoding.UTF8.GetBytes(Message);
            client.WriteAsync(buffer, 0, buffer.Length);
            client.Flush();

            var streamReader = new StreamReader(client);
            while (true)
            {
                var st = streamReader.ReadLine();
                if (st == "end")
                {
                    return;
                }

                Console.WriteLine(st);
            }
        }
    }
}