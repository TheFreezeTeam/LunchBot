namespace DiscordBotSample.Data
{
  using DiscordBotSample.Entities;
  using Microsoft.EntityFrameworkCore;

  public class LunchBotDbContext : DbContext
  {
    public DbSet<Application> Application { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Person> Persons { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
      optionsBuilder.UseSqlite("Data Source=LunchBot.db");
  }
}
