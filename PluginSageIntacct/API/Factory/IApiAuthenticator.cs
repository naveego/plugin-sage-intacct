using System.Threading.Tasks;

namespace PluginSageIntacct.API.Factory
{
    public interface IApiAuthenticator
    {
        Task<string> GetToken();
        Task<string> GetSessionId();
    }
}