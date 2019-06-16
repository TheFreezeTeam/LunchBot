namespace WorkerP6.HostedServices
{
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;
  using System.CommandLine;
  using System.CommandLine.Invocation;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

  internal class CommandLineHostedService : BackgroundService
  {
    private readonly ILogger<CommandLineHostedService> Logger;
    private readonly Parser Parser;

    public CommandLineHostedService(ILogger<CommandLineHostedService> aLogger, Parser aParser)
    {
      Logger = aLogger;
      Parser = aParser;
    }

    protected override async Task ExecuteAsync(CancellationToken aCancellationToken)
    {

      //ClearCurrentConsoleLine();
      bool prompt = true;

      while (prompt)
      {
        Console.Write("timewarp$");
        var args = Console.ReadLine().TrimEnd()?.Split(' ');
        //var args = Prompt.GetString("tangram$", promptColor: ConsoleColor.Cyan)?.TrimEnd()?.Split(' ');

        if (args == null || (args.Length == 1 && string.IsNullOrEmpty(args[0])))
          continue;

        try
        {
          _ = await Parser.InvokeAsync(args);
          //await Execute(args).ConfigureAwait(false);
        }
        catch (Exception e)
        {
          Logger.LogError(e, "something broke");
          //Util.LogException(console, logger, e);
        }
      }

      //await ExitCleanly();
    }
  }
}