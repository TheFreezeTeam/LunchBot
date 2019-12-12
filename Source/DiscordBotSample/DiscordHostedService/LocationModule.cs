namespace DiscordBotSample
{
  using Discord;
  using Discord.Commands;
  using Discord.WebSocket;
  using DiscordBotSample.Data;
  using DiscordBotSample.Entities;
  using System.Threading.Tasks;

  // Create a module with the 'sample' prefix
  [Group("location"),
   Alias("locations")]
  public class LocationModule : ModuleBase<SocketCommandContext>
  {
    public LocationModule(LunchBotDbContext aLunchBotDbContext)
    {
      LunchBotDbContext = aLunchBotDbContext;
    }
    private LunchBotDbContext LunchBotDbContext { get; }

    // location list
    [Command("list")]
    [Summary("List all the locations")]
    public async Task ListLocations()
    {

      foreach (Location location in LunchBotDbContext.Locations)
      {
        string strikeString = location.IsEnabled ? "" : "~~";
        var embedBuilder = new EmbedBuilder
        {
          Title = $"{location.LocationId}. {strikeString}{location.Name}{strikeString}",
          Url = location.Url
        };

        await Context.Channel.SendMessageAsync(null, false, embedBuilder.Build());
      }
    }

    [Command("add")]
    [Summary("")]
    public async Task AddLocationAsync(

      [Summary("The (optional) user to get info from")]
      SocketUser user = null)
    {
      var userInfo = user ?? Context.Client.CurrentUser;
      await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
  }
}
