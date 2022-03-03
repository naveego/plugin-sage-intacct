using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Naveego.Sdk.Plugins;
using Newtonsoft.Json;
using PluginSageIntacct.API.Read;
using PluginSageIntacct.API.Utility;
using PluginSageIntacct.DataContracts;
using PluginSageIntacct.Helper;
using Xunit;
using Record = Naveego.Sdk.Plugins.Record;

namespace PluginSageIntacctTest.Plugin
{
    public class PluginIntegrationTest
    {
        private Settings GetSettings(bool oAuth = false)
        {
            return new Settings
            {
                EndpointUrl = "",
                UserId = "",
                UserPassword = "",
                CompanyId = "",
                SenderId = "",
                SenderPassword = ""
            };
        }

        private ConnectRequest GetConnectSettings(bool oAuth = false)
        {
            var settings = GetSettings(oAuth);
                
            var oAuthConfig = oAuth
                ? new OAuthConfiguration
                {
                    ClientId = "", // add to test
                    ClientSecret = "", // add to test
                }
                : new OAuthConfiguration
                {
                };
            
            var oAuthState = oAuth
                ? new OAuthState
                {
                    RefreshToken = "", // add to test
                    Config = JsonConvert.SerializeObject(new OAuthConfig
                    {
                        RedirectUri = "" // add to test
                    })
                }
                : new OAuthState();
            
            return new ConnectRequest
            {
                SettingsJson = JsonConvert.SerializeObject(settings),
                OauthConfiguration = oAuthConfig,
                OauthStateJson = JsonConvert.SerializeObject(oAuthState)
            };
        }

        private Schema GetTestSchema(string endpointId = null, string id = "test", string name = "test")
        {
            Endpoint endpoint = endpointId == null
                ? EndpointHelper.GetEndpointForId("AllContacts")
                : EndpointHelper.GetEndpointForId(endpointId);


            return new Schema
            {
                Id = id,
                Name = name,
                PublisherMetaJson = JsonConvert.SerializeObject(endpoint),
            };
        }

        [Fact]
        public async Task ConnectSessionTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectSettings();
            var disconnectRequest = new DisconnectRequest();

            // act
            var response = client.ConnectSession(request);
            var responseStream = response.ResponseStream;
            var records = new List<ConnectResponse>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
                client.Disconnect(disconnectRequest);
            }

            // assert
            Assert.Single(records);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ConnectTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectSettings();

            // act
            var response = client.Connect(request);

            // assert
            Assert.IsType<ConnectResponse>(response);
            Assert.Equal("", response.SettingsError);
            Assert.Equal("", response.ConnectionError);
            Assert.Equal("", response.OauthError);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }
        
        [Fact]
        public async Task ConnectOAuthTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var request = GetConnectSettings(true);

            // act
            var response = client.Connect(request);

            // assert
            Assert.IsType<ConnectResponse>(response);
            Assert.Equal("", response.SettingsError);
            Assert.Equal("", response.ConnectionError);
            Assert.Equal("", response.OauthError);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task DiscoverSchemasAllTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectSettings(true);

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.All,
                SampleSize = 10
            };

            // act
            client.Connect(connectRequest);
            var response = client.DiscoverSchemas(request);

            // assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            Assert.Equal(2, response.Schemas.Count);
            //
            var schema = response.Schemas[0];
            Assert.Equal($"AllCustomers", schema.Id);
            Assert.Equal("All Customers", schema.Name);
            Assert.Equal($"", schema.Query);
            Assert.Equal(10, schema.Sample.Count);
            Assert.Equal(204, schema.Properties.Count);
            //
            // var property = schema.Properties[0];
            // Assert.Equal("field1", property.Id);
            // Assert.Equal("field1", property.Name);
            // Assert.Equal("", property.Description);
            // Assert.Equal(PropertyType.String, property.Type);
            // Assert.False(property.IsKey);
            // Assert.True(property.IsNullable);
            //
            // var schema2 = response.Schemas[1];
            // Assert.Equal($"Custom Name", schema2.Id);
            // Assert.Equal("Custom Name", schema2.Name);
            // Assert.Equal($"", schema2.Query);
            // Assert.Equal(10, schema2.Sample.Count);
            // Assert.Equal(17, schema2.Properties.Count);
            //
            // var property2 = schema2.Properties[0];
            // Assert.Equal("field1", property2.Id);
            // Assert.Equal("field1", property2.Name);
            // Assert.Equal("", property2.Description);
            // Assert.Equal(PropertyType.String, property2.Type);
            // Assert.False(property2.IsKey);
            // Assert.True(property2.IsNullable);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task DiscoverSchemasRefreshTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var connectRequest = GetConnectSettings();

            var request = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                SampleSize = 10,
                ToRefresh =
                {
                    GetTestSchema("AllCustomers")
                }
            };

            // act
            client.Connect(connectRequest);
            var response = client.DiscoverSchemas(request);

            // assert
            Assert.IsType<DiscoverSchemasResponse>(response);
            // Assert.Equal(1, response.Schemas.Count);
            //
            // var schema = response.Schemas[0];
            // Assert.Equal("test", schema.Id);
            // Assert.Equal("test", schema.Name);
            // Assert.Equal("", schema.Query);
            // Assert.Equal(10, schema.Sample.Count);
            // Assert.Equal(17, schema.Properties.Count);
            //
            // var property = schema.Properties[0];
            // Assert.Equal("field1", property.Id);
            // Assert.Equal("field1", property.Name);
            // Assert.Equal("", property.Description);
            // Assert.Equal(PropertyType.String, property.Type);
            // Assert.False(property.IsKey);
            // Assert.True(property.IsNullable);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema("AllCustomers");

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = {schema}
            };

            var request = new ReadRequest()
            {
                DataVersions = new DataVersions
                {
                    JobId = "test"
                },
                JobId = "test",
            };

            // act
            client.Connect(connectRequest);
            var schemasResponse = client.DiscoverSchemas(schemaRequest);
            request.Schema = schemasResponse.Schemas[0];

            var response = client.ReadStream(request);
            var responseStream = response.ResponseStream;
            var records = new List<Record>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
            }

            // assert
            Assert.Equal(3914, records.Count);

            var record = JsonConvert.DeserializeObject<Dictionary<string, object>>(records[0].DataJson);
            Assert.Equal("3952", record["RECORDNO"]);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamSubscriberTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema("ActiveSubscribers");

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = {schema}
            };

            var request = new ReadRequest()
            {
                DataVersions = new DataVersions
                {
                    JobId = "test"
                },
                JobId = "test",
            };

            // act
            client.Connect(connectRequest);
            var schemasResponse = client.DiscoverSchemas(schemaRequest);
            request.Schema = schemasResponse.Schemas[0];

            var response = client.ReadStream(request);
            var responseStream = response.ResponseStream;
            var records = new List<Record>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
            }

            // assert
            Assert.Equal(2, records.Count);

            var record = JsonConvert.DeserializeObject<Dictionary<string, object>>(records[0].DataJson);
            // Assert.Equal("~", record["tilde"]);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamLimitTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema();

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = {schema}
            };

            var request = new ReadRequest()
            {
                DataVersions = new DataVersions
                {
                    JobId = "test"
                },
                JobId = "test",
                Limit = 1
            };

            // act
            client.Connect(connectRequest);
            var schemasResponse = client.DiscoverSchemas(schemaRequest);
            request.Schema = schemasResponse.Schemas[0];

            var response = client.ReadStream(request);
            var responseStream = response.ResponseStream;
            var records = new List<Record>();

            while (await responseStream.MoveNext())
            {
                records.Add(responseStream.Current);
            }

            // assert
            Assert.Equal(1, records.Count);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task ReadStreamRealTimeTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema();

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = {schema}
            };

            var request = new ReadRequest()
            {
                DataVersions = new DataVersions
                {
                    JobId = "test",
                    JobDataVersion = 1
                },
                JobId = "test",
                RealTimeStateJson = JsonConvert.SerializeObject(new RealTimeState()),
                RealTimeSettingsJson = JsonConvert.SerializeObject(new RealTimeSettings()),
            };

            // act
            var records = new List<Record>();
            try
            {
                client.Connect(connectRequest);
                var schemasResponse = client.DiscoverSchemas(schemaRequest);
                request.Schema = schemasResponse.Schemas[0];

                var cancellationToken = new CancellationTokenSource();
                cancellationToken.CancelAfter(5000);
                var response = client.ReadStream(request, null, null, cancellationToken.Token);
                var responseStream = response.ResponseStream;


                while (await responseStream.MoveNext())
                {
                    records.Add(responseStream.Current);
                }
            }
            catch (Exception e)
            {
                Assert.Equal("Status(StatusCode=Cancelled, Detail=\"Cancelled\")", e.Message);
            }


            // assert
            Assert.Equal(3, records.Count);

            var record = JsonConvert.DeserializeObject<Dictionary<string, object>>(records[0].DataJson);
            // Assert.Equal("~", record["tilde"]);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }

        [Fact]
        public async Task WriteTest()
        {
            // setup
            Server server = new Server
            {
                Services = {Publisher.BindService(new PluginSageIntacct.Plugin.Plugin())},
                Ports = {new ServerPort("localhost", 0, ServerCredentials.Insecure)}
            };
            server.Start();

            var port = server.Ports.First().BoundPort;

            var channel = new Channel($"localhost:{port}", ChannelCredentials.Insecure);
            var client = new Publisher.PublisherClient(channel);

            var schema = GetTestSchema("AllContacts");

            var connectRequest = GetConnectSettings();

            var schemaRequest = new DiscoverSchemasRequest
            {
                Mode = DiscoverSchemasRequest.Types.Mode.Refresh,
                ToRefresh = {schema}
            };

            var records = new List<Record>()
            {
                {
                    new Record
                    {
                        Action = Record.Types.Action.Upsert,
                        CorrelationId = "test",
                        RecordId = "record1",
                        DataJson = "{\"createdate\":\"2021-05-06T16:55:49.689Z\",\"domain\":\"sample.com\",\"hs_lastmodifieddate\":\"2021-05-06T16:56:10.131Z\",\"hs_object_id\":\"6021949042\",\"name\":\"Updated Sample Company\",\"hs_unique_creation_key\":\"6021949042\"}",
                    }
                },
                {
                    new Record
                    {
                        Action = Record.Types.Action.Upsert,
                        CorrelationId = "test",
                        RecordId = "record2",
                        DataJson = "{\"domain\":\"newsample.com\",\"name\":\"New Sample Company\"}",
                    }
                }
            };

            var recordAcks = new List<RecordAck>();

            // act
            client.Connect(connectRequest);

            var schemasResponse = client.DiscoverSchemas(schemaRequest);

            var prepareWriteRequest = new PrepareWriteRequest()
            {
                Schema = schemasResponse.Schemas[0],
                CommitSlaSeconds = 1000,
                DataVersions = new DataVersions
                {
                    JobId = "jobUnitTest",
                    ShapeId = "shapeUnitTest",
                    JobDataVersion = 1,
                    ShapeDataVersion = 1
                }
            };
            client.PrepareWrite(prepareWriteRequest);

            using (var call = client.WriteStream())
            {
                var responseReaderTask = Task.Run(async () =>
                {
                    while (await call.ResponseStream.MoveNext())
                    {
                        var ack = call.ResponseStream.Current;
                        recordAcks.Add(ack);
                    }
                });

                foreach (Record record in records)
                {
                    await call.RequestStream.WriteAsync(record);
                }

                await call.RequestStream.CompleteAsync();
                await responseReaderTask;
            }

            // assert
            Assert.Equal(2, recordAcks.Count);
            Assert.Equal("", recordAcks[0].Error);
            Assert.Equal("test", recordAcks[0].CorrelationId);

            // cleanup
            await channel.ShutdownAsync();
            await server.ShutdownAsync();
        }
    }
}