using System.Collections.Generic;
using System.IO;

namespace TSQLLint.Common
{
    public interface IPluginContext
    {
        string FilePath { get; }
        
        TextReader FileContents { get; }

        IEnumerable<IRuleException> RuleExceptions { get; }
    }
}
