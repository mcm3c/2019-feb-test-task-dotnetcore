using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestTask {
  public class Program {
    public static void Main(string[] args) {
      CreateHostBuilder(args)
        .Build()
        .Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging => {
              logging.ClearProviders();
              logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder => {
              var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";
              webBuilder
                .UseUrls("http://0.0.0.0:" + port)
                .UseStartup<Startup>();
            });
  }
}
