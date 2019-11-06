using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace BotMama
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Moma.Configure("config.json");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>();
        }
    }
}