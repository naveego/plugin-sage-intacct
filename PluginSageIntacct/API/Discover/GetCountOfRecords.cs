using System.Threading.Tasks;
using Naveego.Sdk.Plugins;
using PluginSageIntacct.API.Factory;
using PluginSageIntacct.API.Utility;

namespace PluginSageIntacct.API.Discover
{
    public static partial class Discover
    {
        public static Task<Count> GetCountOfRecords(IApiClient apiClient, Endpoint? endpoint)
        {
            return endpoint != null
                ? endpoint.GetCountOfRecords(apiClient)
                : Task.FromResult(new Count {Kind = Count.Types.Kind.Unavailable});
        }
    }
}