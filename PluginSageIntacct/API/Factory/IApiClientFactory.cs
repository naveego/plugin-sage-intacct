using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Factory
{
    public interface IApiClientFactory
    {
        IApiClient CreateApiClient(Settings settings);
    }
}