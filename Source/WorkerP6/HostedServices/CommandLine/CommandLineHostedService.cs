namespace WorkerP6.HostedServices
{
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using System;
  using System.CommandLine;
  using System.CommandLine.Invocation;
  using System.Threading;
  using System.Threading.Tasks;

  internal class CommandLineHostedService : BackgroundService
  {
    private readonly ILogger<CommandLineHostedService> Logger;
    private readonly Parser Parser;
    private readonly IAutoCompleteHandler AutoCompleteHandler;

    public CommandLineHostedService(
      ILogger<CommandLineHostedService> aLogger,
      Parser aParser,
      IAutoCompleteHandler aAutoCompleteHandler)
    {
      Logger = aLogger;
      Parser = aParser;
      AutoCompleteHandler = aAutoCompleteHandler;
    }

    protected override async Task ExecuteAsync(CancellationToken aCancellationToken)
    {
      bool loop = true;

      ReadLine.AutoCompletionHandler = AutoCompleteHandler;
      while (loop)
      {
        string prompt = "timewarp$ ";
        string line = ReadLine.Read(prompt).TrimEnd();
        ReadLine.AddHistory(line);
        string[] args = line?.Split(' ');

        if (args == null || (args.Length == 1 && string.IsNullOrEmpty(args[0]))) continue;

        try
        {
          int x = await Parser.InvokeAsync(args);
        }
        catch (Exception e)
        {
          Logger.LogError(e, "Error", args);
        }
      }
    }
  }
}