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
using PluginSageIntacct.DataContracts;
using PluginSageIntacct.Helper;
using PluginSageIntacct.API.Factory;

namespace PluginSageIntacct.API.Utility.EndpointHelperEndpoints
{
    
    
    public class DepartmentsEndpointHelper
    {
        
        private class DepartmentsEndpoint : Endpoint
        {
            

        }

        public static readonly Dictionary<string, Endpoint> DepartmentsEndpoints = new Dictionary<string, Endpoint>
        {
            {
                "AllDepartments", new DepartmentsEndpoint
                {
                    Id = "AllDepartments",
                    Name = "All Departments",
                    LookupObject = "DEPARTMENT",
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
                    "<object>DEPARTMENT</object>" +
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