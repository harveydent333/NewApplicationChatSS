using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NewAppChatSS.DAL
{
    public class DatabaseMigrate
    {
        public static void Migrate(IHost host)
        {
            using var scope = host.Services.CreateScope();

            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetService<NewAppChatSSContext>();
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
