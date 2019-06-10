namespace DiscordBotSample.Data
{
  using DiscordBotSample.Entities;
  using System;
  using System.Linq;
  using DiscordBotSample.Extensions;

  internal class DbInitializer
  {

    public static void Initialize(LunchBotDbContext aLunchBotDbContext)
    {
      aLunchBotDbContext.Database.EnsureCreated();

      if (aLunchBotDbContext.Application.Any())
      {
        return; // DB has already been seeded
      };

      InitializeLocations(aLunchBotDbContext);
      InitializePersons(aLunchBotDbContext);
      InitializeApplication(aLunchBotDbContext);
      aLunchBotDbContext.SaveChanges();
    }

    private static void InitializePersons(LunchBotDbContext aLunchBotDbContext)
    {
      var people = new Person[]
      {
        new Person { Name = "Steve"},
        new Person { Name = "Ross"},
      };
      aLunchBotDbContext.Persons.AddRange(people);
    }

    private static void InitializeLocations(LunchBotDbContext aLunchBotDbContext)
    {
      var locations = new Location[]
      {
        new Location { Name = "Nam Neua 2", Url="https://www.google.com/maps/place/Baan+Num+Neua+Bar/@11.3530552,99.5602715,16.42z/data=!4m5!3m4!1s0x0:0x32ab146768732323!8m2!3d11.354197!4d99.5680846"},
        new Location { Name = "Pla Too Seafood", Url="https://www.google.com/maps/place/Platoo+Seafood/@11.3486113,99.5641838,17.92z/data=!4m5!3m4!1s0x0:0xc33be567c6ad189f!8m2!3d11.3472663!4d99.5652066"},
        new Location { Name = "Ban Sewan", Url="https://www.google.com/maps/place/%E0%B8%84%E0%B8%A3%E0%B8%B1%E0%B8%A7%E0%B8%9A%E0%B9%89%E0%B8%B2%E0%B8%99%E0%B8%AA%E0%B8%A7%E0%B8%99+%E0%B8%9A%E0%B9%89%E0%B8%B2%E0%B8%99%E0%B8%81%E0%B8%A3%E0%B8%B9%E0%B8%94/@11.3608861,99.5578898,15.17z/data=!4m5!3m4!1s0x0:0x390b893e04b593a0!8m2!3d11.3623069!4d99.5588645"},
        new Location { Name = "Ruam Pon", Url="https://www.google.com/maps/place/%E0%B8%A3%E0%B9%89%E0%B8%B2%E0%B8%99%E0%B8%AD%E0%B8%B2%E0%B8%AB%E0%B8%B2%E0%B8%A3%E0%B8%A3%E0%B8%A7%E0%B8%A1%E0%B8%9E%E0%B8%A5/@11.3556358,99.5704748,19.75z/data=!4m5!3m4!1s0x30fee7c318329155:0x4f329ce2301e25a7!8m2!3d11.3558359!4d99.5707702"},
        new Location { Name = "JJs", Url=""},
        new Location { IsEnabled= false, Name = "Cafe Del Mar", Url="https://www.google.com/maps/place/%E0%B8%84%E0%B8%B8%E0%B8%93%E0%B8%99%E0%B8%B2%E0%B8%A2%E0%B8%81%E0%B8%B3%E0%B8%99%E0%B8%B1%E0%B8%99+%E2%80%93+Cafe+del+Mar/@11.3530552,99.5602715,16.42z/data=!4m5!3m4!1s0x0:0x67d9d1fa6a389fa6!8m2!3d11.3559418!4d99.5708553"},
      };
      aLunchBotDbContext.Locations.AddRange(locations);
    }

    private static void InitializeApplication(LunchBotDbContext aLunchBotDbContext)
    {
      var application = new Application
      {
        Name = "Lunch Bot",
        Version = "1.0.0",
        WhoPays = aLunchBotDbContext.Persons.Local.First(aPerson => aPerson.Name == "Ross"),
        NextLocation = aLunchBotDbContext.Locations.Local.First(aLocation => aLocation.Name == "Nam Neua 2"),
        NextTime = DateTime.Today.AddHours(12).GetNextWeekday(DayOfWeek.Friday)
      };

      aLunchBotDbContext.Application.Add(application);
    }
  }
}
