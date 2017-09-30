using System.IO;

namespace TSQLLint.Common
{
    public interface IPluginContext
    {
        string FilePath { get; }
        TextReader FileContents { get; }
    }
}
