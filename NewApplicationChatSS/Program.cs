using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NewAppChatSS.DAL;

namespace NewApplicationChatSS
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var host = CreateHostBuilder(args).Build();
                DatabaseMigrate.Migrate(host);
                await host.RunAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>()
                    .UseDefaultServiceProvider(options =>
                    options.ValidateScopes = false);
            });
        }
    }
}