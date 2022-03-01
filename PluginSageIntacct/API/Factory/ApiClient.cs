using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Naveego.Sdk.Logging;
using PluginSageIntacct.API.Utility;
using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Factory
{
    public class ApiClient: IApiClient
    {
        private IApiAuthenticator Authenticator { get; set; }
        private static HttpClient Client { get; set; }
        private Settings Settings { get; set; }

        private const string ApiKeyParam = "hapikey";

        
        public ApiClient(HttpClient client, Settings settings)
        {
            Authenticator = new ApiAuthenticator(client, settings);
            Client = client;
            Settings = settings;
            
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> GetSessionId()
        {
            return await Authenticator.GetSessionId();
        }
        
        public async Task TestConnection()
        {
            try
            {
                var sessionId = await Authenticator.GetSessionId();

                if (string.IsNullOrWhiteSpace(sessionId))
                {
                    throw new Exception("Unable to generate session.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public string GetEndpointUrl()
        {
            return Settings.EndpointUrl;
        }

        public Settings GetSettings()
        {
            return Settings;
        }
        public async Task<HttpResponseMessage> GetAsync(string path)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                if (!string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    query[ApiKeyParam] = Settings.ApiKey;
                }
                uriBuilder.Query = query.ToString();
                
                var uri = new Uri(uriBuilder.ToString());
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri,
                };

                if (string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
        
        public async Task<HttpResponseMessage> PostAsync(string path, string xml)
        {
            try
            {
                
                // XDocument xDocument = XDocument.Parse(xml);
                //
                // string xmlRequestBody = xDocument.ToString();
                
                return await Client.PostAsync(Settings.EndpointUrl, new StringContent(xml, Encoding.UTF8, "text/xml"));
                
                //  var token = await Authenticator.GetToken();
                //  var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                //  var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                //
                //  if (!string.IsNullOrWhiteSpace(Settings.ApiKey))
                //  {
                //      query[ApiKeyParam] = Settings.ApiKey;
                //  }
                //  uriBuilder.Query = query.ToString();
                //
                //  var uri = new Uri(uriBuilder.ToString());
                //
                //  var request = new HttpRequestMessage
                //  {
                //      Method = HttpMethod.Post,
                //      RequestUri = uri,
                //      Content = json
                //  };
                //
                // if (string.IsNullOrWhiteSpace(Settings.ApiKey))
                // {
                //     request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // }
                //
                // return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
        
        public async Task<HttpResponseMessage> PutAsync(string path, StringContent json)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                if (!string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    query[ApiKeyParam] = Settings.ApiKey;
                }
                uriBuilder.Query = query.ToString();
                
                var uri = new Uri(uriBuilder.ToString());
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Put,
                    RequestUri = uri,
                    Content = json
                };

                if (string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> PatchAsync(string path, StringContent json)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                if (!string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    query[ApiKeyParam] = Settings.ApiKey;
                }
                uriBuilder.Query = query.ToString();
                
                var uri = new Uri(uriBuilder.ToString());
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Patch,
                    RequestUri = uri,
                    Content = json
                };

                if (string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteAsync(string path)
        {
            try
            {
                var token = await Authenticator.GetToken();
                var uriBuilder = new UriBuilder($"{Constants.BaseApiUrl.TrimEnd('/')}/{path.TrimStart('/')}");
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);

                if (!string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    query[ApiKeyParam] = Settings.ApiKey;
                }
                uriBuilder.Query = query.ToString();
                
                var uri = new Uri(uriBuilder.ToString());
                
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = uri
                };

                if (string.IsNullOrWhiteSpace(Settings.ApiKey))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                return await Client.SendAsync(request);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
    }
}