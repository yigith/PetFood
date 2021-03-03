using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var appIdentityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
                var petFoodContext = scope.ServiceProvider.GetRequiredService<PetFoodContext>();

                // create database if not exists
                await appIdentityDbContext.Database.EnsureCreatedAsync();
                await petFoodContext.Database.EnsureCreatedAsync();

                await PetFoodContextSeed.SeedAsync(petFoodContext);
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
