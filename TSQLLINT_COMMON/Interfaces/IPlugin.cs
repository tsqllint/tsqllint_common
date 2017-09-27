namespace TSQLLINT_COMMON
{
    public interface IPlugin
    {
        void PerformAction(IPluginContext context, IReporter reporter);
    }
}
