using System.IO;

namespace TSQLLINT_COMMON
{
    public interface IPluginContext
    {
        string FilePath { get; }
        TextReader FileContents { get; }
    }
}
