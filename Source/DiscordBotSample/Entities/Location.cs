namespace DiscordBotSample.Entities
{
  public class Location:IEntity
  {
    public int LocationId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public bool IsEnabled { get; set; } = true;

    public int Id => LocationId;
  }
}
