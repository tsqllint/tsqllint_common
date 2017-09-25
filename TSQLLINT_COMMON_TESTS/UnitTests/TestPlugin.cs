using System;
using TSQLLINT_COMMON;

namespace TSQLLINT_COMMON_TESTS
{
    public class TestPlugin : IPlugin
    {

        public IPluginResponse PerformAction(IPluginContext context, IReporter reporter)
        {
            throw new NotImplementedException();
        }
    }
}
