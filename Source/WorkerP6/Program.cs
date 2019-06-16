namespace WorkerP6
{
  using System;
  using System.Collections.Generic;
  using System.CommandLine;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using WorkerP6.CommandLine;
  using WorkerP6.HostedServices;
  using WorkerP6.Services;

  public class Program
  {
    public static async Task Main(string[] aArgumentArray)
    {
      IHostBuilder hostBuilder = CreateHostBuilder(aArgumentArray);
      await hostBuilder.RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] aArgumentArray)
    {
      return Host.CreateDefaultBuilder(aArgumentArray)
        .ConfigureServices((aHostBuilderContext, aServices) =>
        {
          Parser parser = new TimeWarpCommandLineBuilder(aServices).Build();
          aServices.AddSingleton<MyService>();
          aServices.AddSingleton(parser);
          //aServices.AddHostedService<Worker>();
          //aServices.AddHostedService<TimedHostedService>();
          aServices.AddHostedService<CommandLineHostedService>();
        });
    }


    // The source for CreateDefaultBuilder for review
    public static IHostBuilder CreateDefaultBuilder(string[] args)
    {
      var builder = new HostBuilder();

      builder.UseContentRoot(Directory.GetCurrentDirectory());
      builder.ConfigureHostConfiguration(config =>
      {
        config.AddEnvironmentVariables(prefix: "DOTNET_");
        if (args != null)
        {
          config.AddCommandLine(args);
        }
      });

      builder.ConfigureAppConfiguration((hostingContext, config) =>
      {
        var env = hostingContext.HostingEnvironment;

        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
        {
          var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
          if (appAssembly != null)
          {
            config.AddUserSecrets(appAssembly, optional: true);
          }
        }

        config.AddEnvironmentVariables();

        if (args != null)
        {
          config.AddCommandLine(args);
        }
      })
      .ConfigureLogging((hostingContext, logging) =>
      {
        logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        logging.AddConsole();
        logging.AddDebug();
        logging.AddEventSourceLogger();
      })
      .UseDefaultServiceProvider((context, options) =>
      {
        options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
      });

      return builder;
    }
  }
}
