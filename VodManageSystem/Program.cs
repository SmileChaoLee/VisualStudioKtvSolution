using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Net;

using VodManageSystem.Models.DataModels;

namespace VodManageSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope()) 
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var dbContext = services.GetRequiredService<KtvSystemDBContext>();
                    if (dbContext.Song.Any())
                    {
                        logger.LogInformation("Program.cs --> Song has records.");   
                    }
                    else
                    {
                        throw new Exception("Song Table does not have any records.");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogInformation(ex,"Program.cs --> DbContext error.");   
                }

            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                           .UseStartup<Startup>()
                           // .UseUrls("http://localhost:5000")
                           // .UseUrls("http://127.0.0.1:5000")
                           .UseUrls("http://192.168.0.16:5000")
                           // .UseUrls("http://10.0.9.191:5000")
                           .Build();
        }
    }
}
