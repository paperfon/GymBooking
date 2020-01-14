using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymBooking.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GymBooking
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                // For migrating db
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                // For reading/sending the PW
                var config = host.Services.GetRequiredService<IConfiguration>();
                // Need to be added in command/package manager console
                // dotnet user-secrets set "Gym:AdminPW" "LexiconNC19!"
                // it's a json format where Gym{AdminPW: ...}
                var adminPW = config["Gym:AdminPW"];

                try
                {
                    SeedData.InitializeAsync(services, adminPW).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "Seed Fail");
                }
            
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
