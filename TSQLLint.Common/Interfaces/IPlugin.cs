namespace TSQLLint.Common
{
    public interface IPlugin
    {
        void PerformAction(IPluginContext context, IReporter reporter);
    }
}
