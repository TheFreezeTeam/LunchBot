namespace DiscordBotSample.Entities
{
  public class Person :IEntity
  {
    public int PersonId { get; set; }
    public string Name { get; set; }

    public int Id => PersonId;
  }
}
