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
      var startup = new Startup();
      //DiscordSocketClient = new DiscordSocketClient(new DiscordSocketConfig
      //{
      //  // How much logging do you want to see?
      //  LogLevel = LogSeverity.Info,

      //  // If you or another service needs to do anything with messages
      //  // (eg. checking Reactions, checking the content of edited/deleted messages),
      //  // you must set the MessageCacheSize. You may adjust the number as needed.
      //  //MessageCacheSize = 50,

      //  // If your platform doesn't have native WebSockets,
      //  // add Discord.Net.Providers.WS4Net from NuGet,
      //  // add the `using` at the top, and uncomment this line:
      //  //WebSocketProvider = WS4NetProvider.Instance
      //});

      //CommandService = new CommandService(new CommandServiceConfig
      //{
      //  // Again, log level:
      //  LogLevel = LogSeverity.Info,

      //  // There's a few more properties you can set,
      //  // for example, case-insensitive commands.
      //  CaseSensitiveCommands = false,
      //});

      //// Subscribe the logging handler to both the client and the CommandService.
      //DiscordSocketClient.Log += DiscordLogger.Log;
      //CommandService.Log += DiscordLogger.Log;

      // Setup your DI container.
      IServiceCollection serviceCollection = new ServiceCollection();
      ServiceProvider = startup.ConfigureServices(serviceCollection);

      EnsureDatabaseInitialized();
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
      DailyTimeIntervalScheduleBuilder onFriday = DailyTimeIntervalScheduleBuilder.Create()
        .OnDaysOfTheWeek(new DayOfWeek[] { DayOfWeek.Friday });

      ITrigger trigger = TriggerBuilder.Create()
        .WithIdentity(nameof(onFriday), "Lunch Bot")
        //.StartAt(new DateTime(2019, 06, 12, 14, 22, 0, 0))
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