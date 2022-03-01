using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Grpc.Core;
using Naveego.Sdk.Logging;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginSageIntacct.Helper;
using PluginSageIntacct.API.Factory;
using PluginSageIntacct.API.Utility.EndpointHelperEndpoints;
using PluginSageIntacct.DataContracts;
using Type = System.Type;

namespace PluginSageIntacct.API.Utility
{
    public static class EndpointHelper
    {
        private static readonly Dictionary<string, Endpoint> Endpoints = new Dictionary<string, Endpoint>();

        static EndpointHelper()
        {
            CustomersEndpointHelper.CustomersEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            InvoicesEndpointHelper.InvoicesEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            RecurringInvoicesEndpointHelper.RecurringInvoicesEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            ConfigurationItemsEndpointHelper.ConfigurationItemsEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            InventoryEndpointHelper.InventoryEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            ItemsEndpointHelper.ItemsEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            ContactsEndpointHelper.ContactsEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
            DepartmentsEndpointHelper.DepartmentsEndpoints.ToList().ForEach(x => Endpoints.TryAdd(x.Key, x.Value));
        }

        public static Dictionary<string, Endpoint> GetAllEndpoints()
        {
            return Endpoints;
        }

        public static Endpoint? GetEndpointForId(string id)
        {
            return Endpoints.ContainsKey(id) ? Endpoints[id] : null;
        }

        public static Endpoint? GetEndpointForSchema(Schema schema)
        {
            var endpointMetaJson = JsonConvert.DeserializeObject<dynamic>(schema.PublisherMetaJson);
            string endpointId = endpointMetaJson.Id;
            return GetEndpointForId(endpointId);
        }
    }

    public abstract class Endpoint
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string LookupObject { get; set; }= "";
        public string PropertiesPath { get; set; } = "";
        public string BasePath { get; set; } = "";
        public string AllPath { get; set; } = "";
        public string? DetailPath { get; set; }
        public string? DetailPropertyId { get; set; }
        public string AllQuery { get; set; }
        public List<string> PropertyKeys { get; set; } = new List<string>();

        public virtual bool ShouldGetStaticSchema { get; set; } = false;

        protected virtual string WritePathPropertyId { get; set; } = "hs_unique_creation_key";

        public string QueryBody { get; set; } = @"<request>" +
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
                                                "{5}" + 
                                                "</query>" +
                                                "</function>" +
                                                "</content>" +
                                                "</operation>" +
                                                "</request>";
        
        public string ObjectDefinitionLookupBody { get; set; } = @"<request>" +
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
                                                                 "<lookup>" +
                                                                 "<object>" +
                                                                 "{5}" + 
                                                                 "</object>" +
                                                                 "</lookup>" +
                                                                 "</function>" +
                                                                 "</content>" +
                                                                 "</operation>" +
                                                                 "</request>";

        public string ObjectWriteBody { get; set; } = @"<request>" +
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
                                                            "<update>" +
                                                            "<{5}>" +
                                                            "{6}" +
                                                            "</{5}>" +
                                                            "</update>" +
                                                            "</function>" +
                                                            "</content>" +
                                                            "</operation>" +
                                                            "</request>";
        
        protected virtual List<string> RequiredWritePropertyIds { get; set; } = new List<string>
        {
            // "hs_unique_creation_key"
        };

        public List<EndpointActions> SupportedActions { get; set; } = new List<EndpointActions>();

        public virtual Task<Count> GetCountOfRecords(IApiClient apiClient)
        {
            return Task.FromResult(new Count
            {
                Kind = Count.Types.Kind.Unavailable,
            });
        }

        public virtual async IAsyncEnumerable<Record> ReadRecordsAsync(IApiClient apiClient, Schema schema, bool isDiscoverRead = false)
        {
            var settings = apiClient.GetSettings();
            
            var after = "";
            var hasMore = false;

            var offset = 0;
            var fields = "";

            foreach (var field in schema.Properties)
            {
                fields += $"<field>{field.Id}</field>";
            }

            if (string.IsNullOrWhiteSpace(AllQuery))
            {
                throw new Exception("No query found for endpoint: " + Name);
            }

           
            do
            {
                var xmlQuery = string.Format(AllQuery, 
                    settings.SenderId, 
                    settings.SenderPassword,
                    DateTime.Now.ToString(),
                    await apiClient.GetSessionId(),
                    Guid.NewGuid().ToString(),
                    fields,
                    offset.ToString());

                XDocument xDocument = XDocument.Parse(xmlQuery);

                string xmlRequestBody = xDocument.ToString();

                HttpResponseMessage response = await apiClient.PostAsync(settings.EndpointUrl, xmlRequestBody);

                response.EnsureSuccessStatusCode();
            
                string responseBody = await response.Content.ReadAsStringAsync();

                XDocument responseDocument = XDocument.Parse(responseBody);
            
                //make class for read response generically
                XmlSerializer readResponseSerializer = new XmlSerializer(typeof(ReadResponseResponse));
                var readResponse =
                    (ReadResponseResponse) readResponseSerializer.Deserialize((responseDocument.CreateReader()));

                hasMore = readResponse.Operation.Result.Data.Numremaining > 0;
                offset += readResponse.Operation.Result.Data.Count;
                
                foreach (XElement element in responseDocument.Descendants(readResponse.Operation.Result.Data.Listtype))
                {
                    var recordMap = new Dictionary<string, object>();
                    
                    foreach (var property in schema.Properties)
                    {
                        try
                        {
                            switch (property.Type)
                            {
                                case(PropertyType.String):
                                case(PropertyType.Text):
                                    recordMap[property.Id] = element.Descendants(property.Id).FirstOrDefault().Value ?? "";
                                    break;
                                case(PropertyType.Integer):
                                    recordMap[property.Id] = Int32.Parse(element.Descendants(property.Id).FirstOrDefault().Value);
                                    break;
                                case(PropertyType.Bool):
                                    recordMap[property.Id] = Boolean.Parse(element.Descendants(property.Id).FirstOrDefault().Value);
                                    break;
                                case(PropertyType.Datetime):
                                case(PropertyType.Date):
                                    recordMap[property.Id] = DateTime.Parse(element.Descendants(property.Id).FirstOrDefault().Value);
                                    break;
                                default:
                                    recordMap[property.Id] = element.Descendants(property.Id).FirstOrDefault().Value ?? "";
                                    break;
                            }
                        }
                        catch
                        {
                            
                        }
                    }
                    
                    yield return new Record
                    {
                        Action = Record.Types.Action.Upsert,
                        DataJson = JsonConvert.SerializeObject(recordMap)
                    };
                }
            } while (hasMore);
        }

        public virtual async Task<string> WriteRecordAsync(IApiClient apiClient, Schema schema, Record record,
            IServerStreamWriter<RecordAck> responseStream, Endpoint endpoint)
        {
            var recordMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(record.DataJson);

            var settings = apiClient.GetSettings();
            var sessionId = await apiClient.GetSessionId();

            var fields = "";
            foreach (var field in recordMap)
            {
                fields += $"<{field.Key}>{field.Value}</{field.Key}>";
            }

            var xmlQuery = string.Format(ObjectWriteBody,
                settings.SenderId,
                settings.SenderPassword,
                DateTime.Now.ToString(),
                sessionId,
                Guid.NewGuid().ToString(),
                endpoint.LookupObject,
                fields);

            XDocument xDocument = XDocument.Parse(xmlQuery);
            string xmlRequestBody = xDocument.ToString();
            HttpResponseMessage response = await apiClient.PostAsync(settings.EndpointUrl, xmlRequestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                
                // if error corresponds to record not existing, create record instead

                var errorAck = new RecordAck
                {
                    CorrelationId = record.CorrelationId,
                    Error = errorMessage
                };
                await responseStream.WriteAsync(errorAck);
            
                return errorMessage;
            }
            
            var ack = new RecordAck
            {
                CorrelationId = record.CorrelationId,
                Error = ""
            };
            await responseStream.WriteAsync(ack);

            return "";
        }

        public virtual Task<Schema> GetStaticSchemaAsync(IApiClient apiClient, Schema schema)
        {
            throw new NotImplementedException();
        }

        public virtual Task<bool> IsCustomProperty(IApiClient apiClient, string propertyId)
        {
            return Task.FromResult(false);
        }

        public Schema.Types.DataFlowDirection GetDataFlowDirection()
        {
            if (CanRead() && CanWrite())
            {
                return Schema.Types.DataFlowDirection.ReadWrite;
            }

            if (CanRead() && !CanWrite())
            {
                return Schema.Types.DataFlowDirection.Read;
            }

            if (!CanRead() && CanWrite())
            {
                return Schema.Types.DataFlowDirection.Write;
            }

            return Schema.Types.DataFlowDirection.Read;
        }


        private bool CanRead()
        {
            return SupportedActions.Contains(EndpointActions.Get);
        }

        private bool CanWrite()
        {
            return SupportedActions.Contains(EndpointActions.Post) ||
                   SupportedActions.Contains(EndpointActions.Put) ||
                   SupportedActions.Contains(EndpointActions.Delete);
        }
    }
    
    public enum EndpointActions
    {
        Get,
        Post,
        Put,
        Delete
    }
}