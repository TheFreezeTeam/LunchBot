namespace DefaultConsole
{
  using System.Threading.Tasks;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  class Program
  {
    static async Task Main(string[] args)
    {
      IHost host = new HostBuilder()
        //.ConfigureLogging((hostContext, config) =>
        //{
        //  config.AddConsole();
        //  config.AddDebug();
        //})
        .ConfigureHostConfiguration(config =>
        {
          //config.AddEnvironmentVariables();
        })
        .ConfigureAppConfiguration((hostContext, config) =>
        {
          //config.AddJsonFile("appsettings.json", optional: true);
          //config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", optional: true);
          //config.AddCommandLine(args);
        })
        .ConfigureServices((hostContext, services) =>
        {
          //services.AddLogging();
          //services.AddSingleton<MonitorLoop>();
          //services.AddHostedService<TimedHostedService>();
          //services.AddHostedService<ConsumeScopedServiceHostedService>();
          //services.AddScoped<IScopedProcessingService, ScopedProcessingService>();
          //services.AddHostedService<QueuedHostedService>();
          //services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
        })
        .UseConsoleLifetime()
        .Build();

      using (host)
      {
        // Start the host
        await host.StartAsync();

        // Monitor for new background queue work items
        //var monitorLoop = host.Services.GetRequiredService<MonitorLoop>();
        //monitorLoop.StartMonitorLoop();

        // Wait for the host to shutdown
        await host.WaitForShutdownAsync();
      }
    }
  }
}
