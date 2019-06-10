namespace DiscordBotSample.Scheduling
{
  using Discord;
  using Discord.Commands;
  using Discord.WebSocket;
  using DiscordBotSample.Data;
  using DiscordBotSample.Entities;
  using DiscordBotSample.Extensions;
  using Microsoft.EntityFrameworkCore;
  using Quartz;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  internal class AfterLunchJob : IJob
  {
    private LunchBotDbContext LunchBotDbContext { get; set; }
    private DiscordSocketClient DiscordSocketClient { get; set; }

    public AfterLunchJob(LunchBotDbContext aLunchBotDbContext, DiscordSocketClient aDiscordSocketClient)
    {
      LunchBotDbContext = aLunchBotDbContext;
      DiscordSocketClient = aDiscordSocketClient;
    }

    public async Task Execute(IJobExecutionContext aContext)
    {
      Application application = LunchBotDbContext.Application
        .Include(a => a.NextLocation)
        .Include(a => a.WhoPays)
        .First();
      if (DateTime.Now > application.NextTime)
      {
        var locations = LunchBotDbContext.Locations.ToList();
        var persons = LunchBotDbContext.Persons.ToList();
        application.NextLocation = locations.Random();
        application.NextTime.GetNextWeekday(DayOfWeek.Friday);
        application.WhoPays = persons.NextAfter(application.WhoPays);
        await LunchBotDbContext.SaveChangesAsync();
      }
      string message = $"Hope you enjoyed your meal. Next lunch will be {application.NextTime:dddd, MMMM dd, yyyy} at {application.NextLocation?.Name}. It will be {application.WhoPays?.Name}'s turn to pay.";

      var embedBuilder = new EmbedBuilder
      {
        Title = $"{application.NextLocation.Name}",
        Url = application.NextLocation.Url
      };

      ulong channelId = 586411887437283328;
      //ulong serverId = 408473039643213826;
      var messageChannel = DiscordSocketClient.GetChannel(channelId) as IMessageChannel;
      if (messageChannel == null)
      {
        Console.WriteLine("WTF");
      }
      else
      {
        _ = await messageChannel.SendMessageAsync(message, false, embedBuilder.Build());
      }
    }
  }
}