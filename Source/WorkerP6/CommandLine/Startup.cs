namespace WorkerP6.CommandLine
{
  using System.CommandLine.Builder;
  using System.CommandLine.Invocation;
  using FluentValidation;
  using MediatR.Pipeline;
  using Microsoft.Extensions.DependencyInjection;
  using WorkerP6.CommandLine.Behaviors;
  using WorkerP6.CommandLine.Commands.Sample;

  internal class Startup
  {
    public void Configure(TimeWarpCommandLineBuilder aTimeWarpCommandLineBuilder)
    {
      aTimeWarpCommandLineBuilder
        .UseVersionOption()
        // middleware
        .UseHelp()
        .UseParseDirective()
        .UseDebugDirective()
        .UseSuggestDirective()
        .RegisterWithDotnetSuggest()
        .UseParseErrorReporting()
        .UseExceptionHandler();
    }

    public void ConfigureServices(IServiceCollection aServiceCollection)
    {
      aServiceCollection.AddScoped(typeof(IRequestPreProcessor<>), typeof(ValidationBehavior<>));
      aServiceCollection.AddScoped(typeof(IValidator<SampleRequest>), typeof(SampleValidator));
      aServiceCollection.AddLogging();
    }
  }
}