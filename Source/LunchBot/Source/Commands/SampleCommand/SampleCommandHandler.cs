namespace LunchBot.Commands.Sample
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using MediatR;

  internal class SampleCommandHandler
    : IRequestHandler<SampleRequest>
  {
    public Task<Unit> Handle(SampleRequest aSampleCommandRequest, CancellationToken aCancellationToken)
    {
      Console.WriteLine($"Parameter1:{aSampleCommandRequest.Parameter1}");
      Console.WriteLine($"Parameter1:{aSampleCommandRequest.Parameter2}");
      Console.WriteLine($"Parameter1:{aSampleCommandRequest.Parameter3}");
      return Unit.Task;
    }
  }
}
