namespace DiscordBotSample.Scheduling
{
  using Microsoft.Extensions.DependencyInjection;
  using Quartz;
  using Quartz.Spi;
  using System;

  class JobFactory : IJobFactory
  {
    protected readonly IServiceProvider ServiceProvider;

    public JobFactory(IServiceProvider aServiceProvider)
    {
      ServiceProvider = aServiceProvider;
    }

    public IJob NewJob(TriggerFiredBundle aBundle, IScheduler aScheduler) =>
      ServiceProvider.GetRequiredService(aBundle.JobDetail.JobType) as IJob;

    public void ReturnJob(IJob aJob) { }
  }
}
