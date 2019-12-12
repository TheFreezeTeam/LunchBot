namespace DiscordBotSample.DiscordHostedService
{
  using Discord.Commands;
  using Discord.WebSocket;
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;

  public class DiscordHostedService
  {

    private readonly CommandService CommandService;

    private readonly DiscordSocketClient DiscordSocketClient;

    private readonly IServiceProvider ServiceProvider;

    public DiscordHostedService(CommandService commandService, DiscordSocketClient discordSocketClient, IServiceProvider serviceProvider)
    {
      CommandService = commandService;
      DiscordSocketClient = discordSocketClient;
      ServiceProvider = serviceProvider;
    }

    private async Task HandleCommandAsync(SocketMessage aSocketMessage)
    {
      // Bail out if it's a System Message.
      var msg = aSocketMessage as SocketUserMessage;
      if (msg == null) return;

      // We don't want the bot to respond to itself or other bots.
      if (msg.Author.Id == DiscordSocketClient.CurrentUser.Id || msg.Author.IsBot) return;

      // Create a number to track where the prefix ends and the command begins
      int pos = 0;
      // Replace the '!' with whatever character
      // you want to prefix your commands with.
      // Uncomment the second half if you also want
      // commands to be invoked by mentioning the bot instead.
      if (msg.HasCharPrefix('!', ref pos) || msg.HasMentionPrefix(DiscordSocketClient.CurrentUser, ref pos))
      {
        // Create a Command Context.
        var context = new SocketCommandContext(DiscordSocketClient, msg);

        // Execute the command. (result does not indicate a return value,
        // rather an object stating if the command executed successfully).
        var result = await CommandService.ExecuteAsync(context, pos, ServiceProvider);

        // Uncomment the following lines if you want the bot
        // to send a message if it failed.
        // This does not catch errors from commands with 'RunMode.Async',
        // subscribe a handler for '_commands.CommandExecuted' to see those.
        if (!result.IsSuccess) //&& result.Error != CommandError.UnknownCommand)
          await msg.Channel.SendMessageAsync(result.ErrorReason);
      }
    }

    private async Task InitCommands()
    {
      // Either search the program and add all Module classes that can be found.
      // Module classes MUST be marked 'public' or they will be ignored.
      // You also need to pass your 'IServiceProvider' instance now,
      // so make sure that's done before you get here.
      await CommandService.AddModulesAsync(Assembly.GetEntryAssembly(), ServiceProvider);
      // Or add Modules manually if you prefer to be a little more explicit:
      //await _commands.AddModuleAsync<SomeModule>(_services);
      // Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

      // Subscribe a handler to see if a message invokes a command.
      DiscordSocketClient.MessageReceived += HandleCommandAsync;
    }

  }
}
