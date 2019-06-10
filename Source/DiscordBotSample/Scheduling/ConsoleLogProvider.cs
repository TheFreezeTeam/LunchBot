﻿namespace DiscordBotSample.Scheduling
{
  using Quartz.Logging;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public  class ConsoleLogProvider : ILogProvider
  {
    public Logger GetLogger(string name)
    {
      return (level, func, exception, parameters) =>
      {
        if (level >= LogLevel.Info && func != null)
        {
          Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
        }
        return true;
      };
    }

    public IDisposable OpenNestedContext(string message)
    {
      throw new NotImplementedException();
    }

    public IDisposable OpenMappedContext(string key, string value)
    {
      throw new NotImplementedException();
    }
  }
}
