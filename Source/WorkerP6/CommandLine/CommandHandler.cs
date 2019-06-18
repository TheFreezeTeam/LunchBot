namespace WorkerP6.CommandLine
{
  using MediatR;
  using System;
  using System.Collections.Generic;
  using System.CommandLine;
  using System.CommandLine.Invocation;
  using System.Reflection;
  using System.Threading.Tasks;

  internal class MediatorCommandHandler : ICommandHandler
  {
    private IMediator Mediator { get; }

    private Type Type { get; }

    public MediatorCommandHandler(Type aType, IMediator aMediator)
    {
      Type = aType;
      Mediator = aMediator;
    }

    //https://stackoverflow.com/questions/1089123/setting-a-property-by-reflection-with-a-string-value
    public async Task<int> InvokeAsync(InvocationContext aInvocationContext)
    {
      try
      {
        ParseResult parseResult = aInvocationContext.ParseResult;
        var request = (IRequest)Activator.CreateInstance(Type);
        foreach (SymbolResult symbolResult in parseResult.CommandResult.Children)
        {
          var optionResult = symbolResult as OptionResult;
          PropertyInfo propertyInfo = Type.GetProperty(symbolResult.Name);
          object result = parseResult.FindResultFor(optionResult.Option).GetValueOrDefault();
          propertyInfo.SetValue(request, result);
        }

        await Mediator.Send(request);

        return 0;
      }
      catch (Exception)
      {
        return 1;
      }
    }
  }
}