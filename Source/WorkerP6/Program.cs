namespace WorkerP6
{
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using System;
  using System.CommandLine;
  using System.IO;
  using System.Reflection;
  using System.Threading.Tasks;
  using WorkerP6.CommandLine;
  using WorkerP6.HostedServices;
  using WorkerP6.HostedServices.CommandLine;
  using WorkerP6.Services;

  internal class Program
  {
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

      builder.ConfigureAppConfiguration((aHostBuilderContext, aConfigurationBuilder) =>
      {
        IHostEnvironment env = aHostBuilderContext.HostingEnvironment;

        aConfigurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

        if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
        {
          var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
          if (appAssembly != null)
          {
            aConfigurationBuilder.AddUserSecrets(appAssembly, optional: true);
          }
        }

        aConfigurationBuilder.AddEnvironmentVariables();

        if (args != null)
        {
          aConfigurationBuilder.AddCommandLine(args);
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

    internal static IHostBuilder CreateHostBuilder(string[] aArgumentArray)
    {
      return Host.CreateDefaultBuilder(aArgumentArray)
        .ConfigureServices((aHostBuilderContext, aServiceCollection) =>
        {
          var timeWarpCommandLineBuilder = new TimeWarpCommandLineBuilder(aServiceCollection);
          Parser parser = timeWarpCommandLineBuilder.Build();
          aServiceCollection.AddSingleton<MyService>();
          aServiceCollection.AddSingleton(parser);
          aServiceCollection.AddSingleton<IAutoCompleteHandler, AutoCompleteHandler>();
          //aServices.AddHostedService<Worker>();
          //aServices.AddHostedService<TimedHostedService>();
          aServiceCollection.AddHostedService<CommandLineHostedService>();
        });
    }

    internal static async Task Main(string[] aArgumentArray)
    {
      IHostBuilder hostBuilder = CreateHostBuilder(aArgumentArray);
      await hostBuilder.RunConsoleAsync();
    }
  }
}