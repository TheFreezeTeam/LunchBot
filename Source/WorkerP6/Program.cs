namespace WorkerP6
{
  using System;
  using System.CommandLine;
  using System.IO;
  using System.Reflection;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using WorkerP6.CommandLine;
  using WorkerP6.HostedServices;
  using WorkerP6.HostedServices.CommandLine;
  using WorkerP6.Services;

  internal class Program
  {
    internal static async Task Main(string[] aArgumentArray)
    {
      IHostBuilder hostBuilder = CreateHostBuilder(aArgumentArray);
      await hostBuilder.RunConsoleAsync();
    }

    internal static IHostBuilder CreateHostBuilder(string[] aArgumentArray)
    {
      return Host.CreateDefaultBuilder(aArgumentArray)
        .ConfigureServices((aHostBuilderContext, aServices) =>
        {
          var timeWarpCommandLineBuilder = new TimeWarpCommandLineBuilder(aServices);
          Parser parser = timeWarpCommandLineBuilder.Build();
          aServices.AddSingleton<MyService>();
          aServices.AddSingleton(parser);
          aServices.AddSingleton<IAutoCompleteHandler, AutoCompleteHandler>();
          //aServices.AddHostedService<Worker>();
          //aServices.AddHostedService<TimedHostedService>();
          aServices.AddHostedService<CommandLineHostedService>();
        });
    }


    // The source for CreateDefaultBuilder for review
    internal static IHostBuilder CreateDefaultBuilder(string[] args)
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
