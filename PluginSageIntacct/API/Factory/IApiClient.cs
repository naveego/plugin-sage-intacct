using System.Net.Http;
using System.Threading.Tasks;
using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Factory
{
    public interface IApiClient
    {
        Task TestConnection();

        string GetEndpointUrl();
        Settings GetSettings();
        Task<string> GetSessionId();
        Task<HttpResponseMessage> GetAsync(string path);
        // Task<HttpResponseMessage> PostAsync(string path, string json);
        Task<HttpResponseMessage> PostAsync(string path, string xml);
        Task<HttpResponseMessage> PutAsync(string path, StringContent json);
        Task<HttpResponseMessage> PatchAsync(string path, StringContent json);
        Task<HttpResponseMessage> DeleteAsync(string path);
    }
}