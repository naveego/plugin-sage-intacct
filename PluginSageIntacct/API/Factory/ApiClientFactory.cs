using System.Net.Http;
using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Factory
{
    public class ApiClientFactory: IApiClientFactory
    {
        private HttpClient Client { get; set; }

        public ApiClientFactory(HttpClient client)
        {
            Client = client;
        }

        public IApiClient CreateApiClient(Settings settings)
        {
            return new ApiClient(Client, settings);
        }
    }
}