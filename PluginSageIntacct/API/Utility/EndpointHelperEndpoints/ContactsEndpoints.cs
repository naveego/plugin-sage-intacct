using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PluginSageIntacct.API.Factory;
using PluginSageIntacct.DataContracts;
using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Utility.EndpointHelperEndpoints
{
    
    
    public class ContactsEndpointHelper
    {
        
        private class ContactsEndpoint : Endpoint
        {
            

        }
        

        public static readonly Dictionary<string, Endpoint> ContactsEndpoints = new Dictionary<string, Endpoint>
        {
            {
                "AllContacts", new ContactsEndpoint
                {
                    Id = "AllContacts",
                    Name = "All Contacts",
                    LookupObject = "CONTACT",
                    SupportedActions = new List<EndpointActions>
                    {
                        EndpointActions.Get,
                        EndpointActions.Post
                    },
                    AllQuery = 
                    "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                    @"<request>" +
                    "<control>" +
                    "<senderid>{0}</senderid>" +
                    "<password>{1}</password>" +
                    "<controlid>{2}</controlid>" +
                    "<uniqueid>false</uniqueid>" +
                    "<dtdversion>3.0</dtdversion>" +
                    "<includewhitespace>false</includewhitespace>" +
                    "</control>" +
                    "<operation>" +
                    "<authentication>" +
                    "<sessionid>{3}</sessionid>" +
                    "</authentication>" +
                    "<content>" +
                    "<function controlid=\"{4}\">" +
                    "<query> " +
                    "<object>CONTACT</object>" +
                    "<select>" +
                    "{5}" +
                    "</select>" +
                    "<offset>{6}</offset>" +
                    "</query>" +
                    "</function>" +
                    "</content>" +
                    "</operation>" +
                    "</request>"
                    
                }
            }
        };
    }
}