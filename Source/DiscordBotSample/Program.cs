namespace DiscordBotSample
{
  using Discord;
  using Discord.Commands;
  using Discord.WebSocket;
  using DiscordBotSample.Data;
  using DiscordBotSample.Scheduling;
  using Microsoft.Extensions.DependencyInjection;
  using Quartz;
  using Quartz.Impl;
  using Quartz.Logging;
  using System;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;

  internal class Program
  {
    // Keep the CommandService and DI container around for use with commands.
    // These two types require you install the Discord.Net.Commands package.
    private readonly CommandService CommandService;

    private readonly DiscordSocketClient DiscordSocketClient;

    private readonly IServiceProvider ServiceProvider;

    private Program()
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
      DiscordSocketClient.Log += Log;
      CommandService.Log += Log;

      // Setup your DI container.
      ServiceProvider = ConfigureServices();

      EnsureDatabaseInitialized();
    }

    // If any services require the client, or the CommandService, or something else you keep on hand,
    // pass them as parameters into this method as needed.
    // If this method is getting pretty long, you can separate it out into another file using partials.
    private IServiceProvider ConfigureServices()
    {
      IServiceCollection serviceCollection = new ServiceCollection()
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
      return serviceCollection.BuildServiceProvider(true);
    }

    // Example of a logging handler. This can be re-used by addons
    // that ask for a Func<LogMessage, Task>.
    private static Task Log(LogMessage aLogMessage)
    {
      switch (aLogMessage.Severity)
      {
        case LogSeverity.Critical:
        case LogSeverity.Error:
          Console.ForegroundColor = ConsoleColor.Red;
          break;

        case LogSeverity.Warning:
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;

        case LogSeverity.Info:
          Console.ForegroundColor = ConsoleColor.White;
          break;

        case LogSeverity.Verbose:
        case LogSeverity.Debug:
          Console.ForegroundColor = ConsoleColor.DarkGray;
          break;
      }
      Console.WriteLine($"{DateTime.Now,-19} [{aLogMessage.Severity,8}] {aLogMessage.Source}: {aLogMessage.Message} {aLogMessage.Exception}");
      Console.ResetColor();

      // If you get an error saying 'CompletedTask' doesn't exist,
      // your project is targeting .NET 4.5.2 or lower. You'll need
      // to adjust your project's target framework to 4.6 or higher
      // (instructions for this are easily Googled).
      // If you *need* to run on .NET 4.5 for compat/other reasons,
      // the alternative is to 'return Task.Delay(0);' instead.
      return Task.CompletedTask;
    }

    // Program entry point
    private static void Main(string[] args)
    {
      LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
      // Call the Program constructor, followed by the
      // MainAsync method and wait until it finishes (which should be never).
      new Program().MainAsync().GetAwaiter().GetResult();
    }

    private async Task ConfigureScheduler()
    {
      var jobFactory = new JobFactory(ServiceProvider);
      
      var schedulerFactory = new StdSchedulerFactory();
      IScheduler scheduler = await schedulerFactory.GetScheduler();
      scheduler.JobFactory = jobFactory; // Use our DI container to create the Job
      
      await scheduler.Start();

      IJobDetail job = JobBuilder.Create<AfterLunchJob>()
          .WithIdentity(nameof(AfterLunchJob), "Lunch Bot")
          .Build();

      // Trigger the job every Friday sometime after Lunch  ....say 3:00 
      var onFriday = DailyTimeIntervalScheduleBuilder.Create()
        .OnDaysOfTheWeek(new DayOfWeek[] { DayOfWeek.Friday });

      ITrigger trigger = TriggerBuilder.Create()
        .WithIdentity(nameof(onFriday), "Lunch Bot")
        .WithSchedule(onFriday)
        .Build();

      // Tell quartz to schedule the job using our trigger
      await scheduler.ScheduleJob(job, trigger);
    }

    private void EnsureDatabaseInitialized()
    {
      using (IServiceScope scope = ServiceProvider.CreateScope())
      {
        IServiceProvider services = scope.ServiceProvider;
        try
        {
          LunchBotDbContext lunchBotDbContext = services.GetRequiredService<LunchBotDbContext>();
          DbInitializer.Initialize(lunchBotDbContext);
        }
        catch (Exception exception)
        {
          Console.WriteLine(exception.Message);
          throw;
        }
      }
    }

    private async Task HandleCommandAsync(SocketMessage aSocketMessage)
    {
      // Bail out if it's a System Message.
      var msg = aSocketMessage as SocketUserMessage;
      if (msg == null) return;

      // We don't want the bot to respond to itself or other bots.
      if (msg.Author.Id == DiscordSocketClient.CurrentUser.Id || msg.Author.IsBot) return;

      // Create a number to track where the prefix ends and the command begins
      int pos = 0;
      // Replace the '!' with whatever character
      // you want to prefix your commands with.
      // Uncomment the second half if you also want
      // commands to be invoked by mentioning the bot instead.
      if (msg.HasCharPrefix('!', ref pos) || msg.HasMentionPrefix(DiscordSocketClient.CurrentUser, ref pos))
      {
        // Create a Command Context.
        var context = new SocketCommandContext(DiscordSocketClient, msg);

        // Execute the command. (result does not indicate a return value,
        // rather an object stating if the command executed successfully).
        var result = await CommandService.ExecuteAsync(context, pos, ServiceProvider);

        // Uncomment the following lines if you want the bot
        // to send a message if it failed.
        // This does not catch errors from commands with 'RunMode.Async',
        // subscribe a handler for '_commands.CommandExecuted' to see those.
        if (!result.IsSuccess) //&& result.Error != CommandError.UnknownCommand)
          await msg.Channel.SendMessageAsync(result.ErrorReason);
      }
    }

    private async Task InitCommands()
    {
      // Either search the program and add all Module classes that can be found.
      // Module classes MUST be marked 'public' or they will be ignored.
      // You also need to pass your 'IServiceProvider' instance now,
      // so make sure that's done before you get here.
      await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
      // Or add Modules manually if you prefer to be a little more explicit:
      //await _commands.AddModuleAsync<SomeModule>(_services);
      // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

      // Subscribe a handler to see if a message invokes a command.
      DiscordSocketClient.MessageReceived += HandleCommandAsync;
    }

    private async Task MainAsync()
    {

      // Centralize the logic for commands into a separate method.
      await InitCommands();

      // Login and connect.
      await DiscordSocketClient.LoginAsync(TokenType.Bot,
          // < DO NOT HARDCODE YOUR TOKEN >
          Environment.GetEnvironmentVariable("DiscordToken"));
      await DiscordSocketClient.StartAsync();
      
      await ConfigureScheduler();
      // Wait infinitely so your bot actually stays connected.
      await Task.Delay(Timeout.Infinite);
    }
  }
}