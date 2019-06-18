using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkerP6.HostedServices
{
  /// <summary>
  /// Not used currently.  Though is maybe I will want to capture input and out.
  /// </summary>
  public class TimeWarpConsole : IConsole
  {
    public IStandardStreamWriter Out => throw new NotImplementedException();

    public bool IsOutputRedirected => throw new NotImplementedException();

    public IStandardStreamWriter Error => throw new NotImplementedException();

    public bool IsErrorRedirected => throw new NotImplementedException();

    public bool IsInputRedirected => throw new NotImplementedException();

  }
}
