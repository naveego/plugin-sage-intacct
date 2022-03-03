using System;

namespace PluginSageIntacct.Helper
{
    public class Settings
    {

        public string CompanyId { get; set; }
        public string UserId { get; set; }
        public string UserPassword { get; set; }
        
        public string EndpointUrl { get; set; }
        public string SenderId { get; set; }
        public string SenderPassword { get; set; }
        
        
        public string ClientId { get; set; }
        public string EntityId { get; set; }

        //old below - remove when convenient}
        public string ApiKey { get; set; }
        public string ClientSecret { get; set; }
        public string RefreshToken { get; set; }
        public string RedirectUri { get; set; }

        /// <summary>
        /// Validates the settings input object
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Validate()
        {
            if (String.IsNullOrEmpty(CompanyId))
            {
                throw new Exception("the CompanyId property must be set");
            }
            if (String.IsNullOrEmpty(UserId))
            {
                throw new Exception("the UserId property must be set");
            }
            if (String.IsNullOrEmpty(UserPassword))
            {
                throw new Exception("the UserPassword property must be set");
            }
            if (String.IsNullOrEmpty(EndpointUrl))
            {
                throw new Exception("the EndpointUrl property must be set");
            }
            if (String.IsNullOrEmpty(SenderId))
            {
                throw new Exception("the SenderId property must be set");
            }
            if (String.IsNullOrEmpty(SenderPassword))
            {
                throw new Exception("the SenderPassword property must be set");
            }
        }
    }
}