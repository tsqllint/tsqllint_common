using System.Collections.Generic;

namespace TSQLLINT_COMMON
{
    public interface IPluginResponse
    {
        int Errors { get; }
        int Warnings { get; }
        IList<string> Messages { get; }
    }
}
