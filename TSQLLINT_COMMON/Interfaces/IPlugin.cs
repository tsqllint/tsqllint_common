namespace TSQLLINT_COMMON
{
    public interface IPlugin
    {
        IPluginResponse PerformAction(IPluginContext context);
    }
}
