namespace WorkerP6.HostedServices
{
  using System;
  using System.Collections.Generic;
  using System.CommandLine;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using System.CommandLine.Invocation;

  public class Worker : BackgroundService
  {
    private readonly ILogger<Worker> Logger;
    private readonly Parser Parser;

    public Worker(ILogger<Worker> aLogger, Parser aParser)
    {
      Logger = aLogger;
      Parser = aParser;
    }

    protected override async Task ExecuteAsync(CancellationToken aCancellationToken)
    {
      if (aCancellationToken.IsCancellationRequested)
      {
        Logger.LogInformation("Cancellation requested shutting down");
      }
      while (!aCancellationToken.IsCancellationRequested)
      {
        Logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        string[] args = { "--help"};
        _ = await Parser.InvokeAsync(args);
        args = new string[] { "SampleCommand","--Parameter1","Bob", "--Parameter2", "2","--Parameter3","6"};
        _ = await Parser.InvokeAsync(args);
        await Task.Delay(3000, aCancellationToken);
      }
    }
  }
}
