using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Naveego.Sdk.Logging;
using Newtonsoft.Json;
using System.Xml.Serialization;
using PluginSageIntacct.DataContracts;
using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Factory
{
    public class ApiAuthenticator: IApiAuthenticator
    {
        private HttpClient Client { get; set; }
        private Settings Settings { get; set; }
        private string Token { get; set; }
        private DateTime ExpiresAt { get; set; }
        
        private string SessionId { get; set; }
        
        private const string AuthUrl = "https://api.hubapi.com/oauth/v1/token"; //settings.endpoint_url

            //<?xml version=""1.0"" encoding=""UTF-8""?>
        private const string AuthenticateSessionQuery = @"
        <request>
          <control>
            <senderid>{0}</senderid>
            <password>{1}</password>
            <controlid>{2}</controlid>
            <uniqueid>false</uniqueid>
            <dtdversion>3.0</dtdversion>
            <includewhitespace>false</includewhitespace>
          </control>
          <operation>
            <authentication>
              <login>
                <userid>{3}</userid>
                <companyid>{4}{5}</companyid>
                <password>{6}</password>
              </login>
            </authentication>
            <content>
              <function controlid=""{7}"">
                <getAPISession />
              </function>
            </content>
          </operation>
        </request>
        ";
        
        
        public ApiAuthenticator(HttpClient client, Settings settings)
        {
            Client = client;
            Settings = settings;
            ExpiresAt = DateTime.Now;
            Token = "";
            SessionId = "";
        }

        public async Task<string> GetSessionId()
        {
            // check if token is expired or will expire in 5 minutes or less
            if (DateTime.Compare(DateTime.Now.AddMinutes(5), ExpiresAt) >= 0 ||
                string.IsNullOrWhiteSpace(SessionId))
            {
                return await GetNewSessionId();
            }

            return SessionId;
        }
        
        public async Task<string> GetToken()
        {
            if (!string.IsNullOrWhiteSpace(Settings.ApiKey))
            {
                return Token;
            }
            
            // check if token is expired or will expire in 5 minutes or less
            if (DateTime.Compare(DateTime.Now.AddMinutes(5), ExpiresAt) >= 0)
            {
                return await GetNewToken();
            }
          
            return Token;
        }

        private async Task<string> GetNewSessionId()
        {
            try
            {

                //var body = new FormUrlEncodedContent(formData);
                    
                var client = Client;
            
                //build query
                var slideIn = "";

                if (!string.IsNullOrWhiteSpace(Settings.ClientId))
                {
                    slideIn = slideIn + "|" + Settings.ClientId;
                }
                if (!string.IsNullOrWhiteSpace(Settings.EntityId))
                {
                    slideIn = slideIn + "|" + Settings.EntityId;
                }
                
                var query = string.Format(AuthenticateSessionQuery, 
                    Settings.SenderId,
                    Settings.SenderPassword,
                    DateTime.Now.ToString(),
                    Settings.UserId,
                    Settings.CompanyId,
                    slideIn,
                    Settings.UserPassword,
                    Guid.NewGuid().ToString()
                     );

                XDocument xDocument = XDocument.Parse(query);

                string xmlRequestBody = xDocument.ToString();
                
                HttpResponseMessage response = await client.PostAsync(Settings.EndpointUrl, new StringContent(xmlRequestBody, Encoding.UTF8, "text/xml"));

                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                XDocument responseDocument = XDocument.Parse(responseBody);
                
                XmlSerializer authResponseSerializer = new XmlSerializer(typeof(AuthResponse));
                var authResponse = (AuthResponse) authResponseSerializer.Deserialize(responseDocument.CreateReader());

                if (!string.IsNullOrWhiteSpace(authResponse.Operation.ErrorMessage?.Error.ErrorNo))
                {
                    var error = authResponse.Operation.ErrorMessage?.Error;
                    throw new Exception($"Auth Error: {error?.ErrorNo} {error?.Description} {error?.Description2}");
                }
                // update expiration and saved token
                ExpiresAt = authResponse.Operation.Authentication.Sessiontimeout;
                SessionId = authResponse.Operation.Result?.Data.Api.Sessionid ?? "";
                
                return SessionId;
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
        
        private async Task<string> GetNewToken()
        {
            try
            {
                var formData = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("client_id", Settings.ClientId),
                    new KeyValuePair<string, string>("client_secret", Settings.ClientSecret),
                    new KeyValuePair<string, string>("refresh_token", Settings.RefreshToken),
                    new KeyValuePair<string, string>("redirect_uri", Settings.RedirectUri)
                };

                var body = new FormUrlEncodedContent(formData);
                    
                var client = Client;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
                var response = await Client.PostAsync(AuthUrl, body);
                response.EnsureSuccessStatusCode();
                    
                var content = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());
                    
                // update expiration and saved token
                ExpiresAt = DateTime.Now.AddSeconds(content.ExpiresIn);
                Token = content.AccessToken;

                return Token;
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }
    }
}