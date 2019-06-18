namespace WorkerP6.HostedServices.CommandLine
{
  using System;
  using System.Collections.Generic;
  using System.CommandLine;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  class AutoCompleteHandler : IAutoCompleteHandler
  {
    private readonly Parser Parser;

    public AutoCompleteHandler(Parser aParser)
    {
      Parser = aParser;
    }

    public char[] Separators { get; set; } = new char[] { ' ', '.', '/', '\\', ':' };
    public string[] GetSuggestions(string aCommandLine, int aIndex)
    {
      ParseResult parseResult = Parser.Parse(aCommandLine);
      return parseResult.Suggestions().ToArray();
    }
  }
}
