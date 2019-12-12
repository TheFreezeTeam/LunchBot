namespace DiscordBotSample
{
  using Dawn;
  using Discord.Commands;
  using DiscordBotSample.Data;
  using DiscordBotSample.Entities;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  [Group("person"),
   Alias("persons", "people")]

  public class PersonModule : ModuleBase<SocketCommandContext>
  {
    public PersonModule(LunchBotDbContext aLunchBotDbContext)
    {
      LunchBotDbContext = aLunchBotDbContext;
    }

    private LunchBotDbContext LunchBotDbContext { get; }


    [Command("list")]
    [Summary("List all person")]
    public async Task ListPersons()
    {
      string message = "";
      foreach (Person person in LunchBotDbContext.Persons)
      {
        message += $"Id:{person.Id} Name:{person.Name} IsEnabled: {person.IsEnabled}\n";
      }

      await Context.Channel.SendMessageAsync(message);
    }

    [Command("add")]
    [Summary("Add a person")]
    public async Task AddPerson([Remainder] [Summary("The name of the new person")] string aName)
    {

      var person = new Person
      {
        Name = Guard.Argument(aName, nameof(aName)).NotNull().NotEmpty()
      };

      LunchBotDbContext.Persons.Add(person);
      LunchBotDbContext.SaveChanges();

      string message = $"Person {person.Name} was added";
      await Context.Channel.SendMessageAsync(message);
    }

    [Command("disable")]
    [Summary("disable a person by Id")]
    public async Task Person([Summary("The id of the person to be disabled")] int aId)
    {
      Guard.Argument(aId, nameof(aId)).NotZero().NotNegative();

      string message;
      Person person = LunchBotDbContext.Persons.FirstOrDefault(aPerson => aPerson.PersonId == aId);
      if (person == null)
      {
        message = "Could not find a person with the Id of {aId}";
      } else if (person.IsEnabled)
      {
        person.IsEnabled = false;
        LunchBotDbContext.SaveChanges();
        message = $"Person {person.Name} was disabled";
      } else
      {
        message = $"Person {person.Name} was already disabled";
      }
      
      await Context.Channel.SendMessageAsync(message);
    }
  }
}
