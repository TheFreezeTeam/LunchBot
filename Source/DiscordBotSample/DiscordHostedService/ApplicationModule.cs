namespace DiscordBotSample
{
  using Discord;
  using Discord.Commands;
  using Discord.WebSocket;
  using DiscordBotSample.Data;
  using DiscordBotSample.Entities;
  using Microsoft.EntityFrameworkCore;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  // Create a module with the 'sample' prefix
  public class ApplicationModule : ModuleBase<SocketCommandContext>
  {
    public ApplicationModule(LunchBotDbContext aLunchBotDbContext)
    {
      LunchBotDbContext = aLunchBotDbContext;
    }
    private LunchBotDbContext LunchBotDbContext { get; }

    // location list
    [Command("status")]
    [Summary("Show next lunch info")]
    public async Task Status()
    {
      Application application = LunchBotDbContext.Application
        .Include(aApplication => aApplication.NextLocation)
        .Include(aApplication => aApplication.WhoPays)
        .First();

      string message = $"Lunch will be {application.NextTime:dddd, MMMM dd, yyyy} at {application.NextLocation.Name}. It will be {application.WhoPays.Name}'s turn to pay.";

      var embedBuilder = new EmbedBuilder
      {
        Title = $"{application.NextLocation.Name}",
        Url = application.NextLocation.Url
      };

      await Context.Channel.SendMessageAsync(message, false, embedBuilder.Build());
    }

    [Command("add")]
    [Summary
    ("Returns info about the current user, or the user parameter, if one passed.")]
    [Alias("user", "whois")]
    public async Task AddLocationAsync(

      [Summary("The (optional) user to get info from")]
      SocketUser user = null)
    {
      var userInfo = user ?? Context.Client.CurrentUser;
      await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
  }
}
