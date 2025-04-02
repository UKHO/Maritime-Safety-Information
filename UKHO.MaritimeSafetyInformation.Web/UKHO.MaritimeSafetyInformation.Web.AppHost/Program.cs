var builder = DistributedApplication.CreateBuilder(args);

var mockcontainer = builder.AddDockerfile("adds-mock", @"..\UKHO.ADDS.Mocks\src\ADDSMock")
    .WithHttpEndpoint(port: 5678, targetPort: 5678, "mock-endpoint");
var mockEndpoint = mockcontainer.GetEndpoint("mock-endpoint");

var storage = builder.AddAzureStorage("storageConnection").RunAsEmulator(
                        azurite =>
                        {
                            azurite.WithDataVolume();
                        });

var tableStorage = storage.AddTables("fss-tables-connection");


var rnwDb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("MSI-RNWDB-1");



var mvcApp = builder.AddProject<Projects.UKHO_MaritimeSafetyInformation_Web>("ukho-msi-web")
    .WithReference(mockEndpoint)
    .WaitFor(mockcontainer)
    .WithReference(rnwDb)
    .WaitFor(rnwDb)
    .WithReference(tableStorage)
    .WaitFor(tableStorage);


mvcApp.WithEnvironment(callback =>
{
    callback.EnvironmentVariables["RadioNavigationalWarningsContext__ConnectionString"] = rnwDb.Resource.ConnectionStringExpression;
    callback.EnvironmentVariables["FileShareService__BaseUrl"] = new UriBuilder(mockEndpoint.Url) { Path = "fss" }.Uri.ToString();
    callback.EnvironmentVariables["CacheConfiguration__ConnectionString"] = tableStorage.Resource.ConnectionStringExpression;
   
});


builder.Build().Run();
