namespace WorkerP6.HostedServices
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;

  internal class TimedHostedService : IHostedService, IDisposable
  {
    private readonly ILogger Logger;
    private Timer Timer;

    public TimedHostedService(ILogger<TimedHostedService> aLogger)
    {
      Logger = aLogger;
    }

    public Task StartAsync(CancellationToken aCancellationToken)
    {
      Logger.LogInformation("Timed Background Service is starting.");

      Timer = new Timer(DoWork, null, TimeSpan.Zero,
          TimeSpan.FromSeconds(5));

      return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
      Logger.LogInformation("Timed Background Service is working.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      Logger.LogInformation("Timed Background Service is stopping.");

      Timer?.Change(Timeout.Infinite, 0);

      return Task.CompletedTask;
    }

    public void Dispose()
    {
      Timer?.Dispose();
    }
  }
 
}