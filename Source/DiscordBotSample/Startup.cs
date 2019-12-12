namespace DiscordBotSample
{
  using Discord;
  using Discord.Commands;
  using Discord.WebSocket;
  using DiscordBotSample.Data;
  using DiscordBotSample.Scheduling;
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public class Startup
  {

    private readonly CommandService CommandService;

    private readonly DiscordSocketClient DiscordSocketClient;

    public Startup()
    {
      DiscordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
      {
        // How much logging do you want to see?
        LogLevel = LogSeverity.Info,

        // If you or another service needs to do anything with messages
        // (eg. checking Reactions, checking the content of edited/deleted messages),
        // you must set the MessageCacheSize. You may adjust the number as needed.
        //MessageCacheSize = 50,

        // If your platform doesn't have native WebSockets,
        // add Discord.Net.Providers.WS4Net from NuGet,
        // add the `using` at the top, and uncomment this line:
        //WebSocketProvider = WS4NetProvider.Instance
      });

      CommandService = new CommandService(new CommandServiceConfig
      {
        // Again, log level:
        LogLevel = LogSeverity.Info,

        // There's a few more properties you can set,
        // for example, case-insensitive commands.
        CaseSensitiveCommands = false,
      });

      // Subscribe the logging handler to both the client and the CommandService.
      DiscordSocketClient.Log += DiscordLogger.Log;
      CommandService.Log += DiscordLogger.Log;
    }

    // If any services require the client, or the CommandService, or something else you keep on hand,
    // pass them as parameters into this method as needed.
    // If this method is getting pretty long, you can separate it out into another file using partials.
    internal IServiceProvider ConfigureServices(IServiceCollection aServiceCollection)
    {
      aServiceCollection
        .AddSingleton<LunchBotDbContext>()
        .AddSingleton<AfterLunchJob>()
        .AddSingleton(DiscordSocketClient)
        .AddSingleton(CommandService);
      //.AddDbContext<LunchBotDbContext>();

      // Repeat this for all the service classes
      // and other dependencies that your commands might need.
      //.AddSingleton(new SomeServiceClass());

      // When all your required services are in the collection, build the container.
      // Tip: There's an overload taking in a 'validateScopes' bool to make sure
      // you haven't made any mistakes in your dependency graph.
      return aServiceCollection.BuildServiceProvider(true);
    }

  }
}
