using System.Text;

namespace BotMama.Cli
{
    internal abstract class CliAnswer
    {
        internal void Run(params string[] args)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(Answer(args) + "\nend\n");
                Program.Server.Write(bytes, 0, bytes.Length);
                Program.Server.Flush();
            }
            catch { }
        }

        protected abstract string Answer(string[] args);
    }
}