namespace DiscordBotSample.Entities
{
  using System;

  public class Application
  {
    public int ApplicationId { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public DateTime NextTime { get; set; }
    public Location NextLocation { get; set; }
    public Person WhoPays { get; set; }

    public int Id => ApplicationId;
  }
}
