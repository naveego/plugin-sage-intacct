using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginSageIntacct.API.Factory;
using PluginSageIntacct.API.Utility;
using PluginSageIntacct.DataContracts;
using PluginSageIntacct.Helper;

namespace PluginSageIntacct.API.Discover
{
    public static partial class Discover
    {
        public static async IAsyncEnumerable<Schema> GetAllSchemas(IApiClient apiClient, Settings settings,
            int sampleSize = 5)
        {
            var allEndpoints = EndpointHelper.GetAllEndpoints();

            foreach (var endpoint in allEndpoints.Values)
            {
                // base schema to be added to
                var schema = new Schema
                {
                    Id = endpoint.Id,
                    Name = endpoint.Name,
                    Description = "",
                    PublisherMetaJson = JsonConvert.SerializeObject(endpoint),
                    DataFlowDirection = endpoint.GetDataFlowDirection()
                };

                schema = await GetSchemaForEndpoint(apiClient, schema, endpoint);

                // get sample and count
                yield return await AddSampleAndCount(apiClient, schema, sampleSize, endpoint);
            }
        }

        private static async Task<Schema> AddSampleAndCount(IApiClient apiClient, Schema schema,
            int sampleSize, Endpoint? endpoint)
        {
            if (endpoint == null)
            {
                return schema;
            }

            // add sample and count
            var records = Read.Read.ReadRecordsAsync(apiClient, schema).Take(sampleSize);
            schema.Sample.AddRange(await records.ToListAsync());
            schema.Count = await GetCountOfRecords(apiClient, endpoint);

            return schema;
        }

        private static async Task<Schema> GetSchemaForEndpoint(IApiClient apiClient, Schema schema, Endpoint? endpoint)
        {
            if (endpoint == null)
            {
                return schema;
            }

            if (endpoint.ShouldGetStaticSchema)
            {
                return await endpoint.GetStaticSchemaAsync(apiClient, schema);
            }
            
            var settings = apiClient.GetSettings();
            var sessionId = await apiClient.GetSessionId();
            var lookupBody = endpoint.ObjectDefinitionLookupBody;

            
            var lookupQuery = string.Format(lookupBody,
                settings.SenderId,
                settings.SenderPassword,
                DateTime.Now.ToString(),
                sessionId,
                Guid.NewGuid().ToString(),
                endpoint.LookupObject);

            XDocument xDocument = XDocument.Parse(lookupQuery);
            string xmlLookupBody = xDocument.ToString();
            
            HttpResponseMessage response = await apiClient.PostAsync(apiClient.GetEndpointUrl(), xmlLookupBody);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            XDocument responseDocument = XDocument.Parse(responseBody);
            XmlSerializer responseSerializer = new XmlSerializer(typeof(ObjectDefinitionResponse));

            var objectDefinitionResponse = (ObjectDefinitionResponse) responseSerializer.Deserialize(responseDocument.CreateReader());
            
            var properties = new List<Property>();

            foreach (var field in objectDefinitionResponse.Operation.Result.Data.Type.Fields.FieldList)
            {
                var propertyMetaJson = new PropertyMetaJson
                {
                    Calculated = false,
                    IsKey = field.REQUIRED,
                };
                
                properties.Add(new Property
                {
                    Id = field.ID,
                    Name = field.LABEL,
                    Description = field.DESCRIPTION,
                    Type = GetPropertyType(field.DATATYPE),
                    TypeAtSource = field.DATATYPE,
                    IsKey = field.REQUIRED,
                    IsNullable = !field.REQUIRED,
                    IsCreateCounter = false,
                    IsUpdateCounter = false,
                    PublisherMetaJson = JsonConvert.SerializeObject(propertyMetaJson),
                });
            }
            
            schema.Properties.Clear();
            schema.Properties.AddRange(properties);
            
            if (schema.Properties.Count == 0)
            {
                schema.Description = Constants.EmptySchemaDescription;
            }
            
            schema.DataFlowDirection = endpoint.GetDataFlowDirection();

            return schema;
        }
    }
}