namespace WorkerP6.CommandLine
{
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  public static class CommandLineServiceCollectionExtensions
  {
    public static IServiceCollection AddCommandLine(this IServiceCollection aServices)
    {

      return aServices;
    }
  }
}
