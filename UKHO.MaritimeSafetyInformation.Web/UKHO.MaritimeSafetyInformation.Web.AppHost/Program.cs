var builder = DistributedApplication.CreateBuilder(args);

var mockcontainer = builder.AddDockerfile("adds-mock", @"..\UKHO.ADDS.Mocks\src\ADDSMock")
    .WithHttpEndpoint(port: 5000, targetPort: 5678, "mock-endpoint");
var mockEndpoint = mockcontainer.GetEndpoint("mock-endpoint");

var storage = builder.AddAzureStorage("storageConnection").RunAsEmulator(
                        azurite =>
                        {
                            azurite.WithDataVolume();
                        });

var tableStorage = storage.AddTables("fss-tables-connection");

var rnwDb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("RadioNavigationalWarningsDb");



builder.AddProject<Projects.UKHO_MaritimeSafetyInformation_Web>("ukho-msi-web")
    .WithReference(mockEndpoint)
    .WaitFor(mockcontainer)
    .WithReference(rnwDb)
    .WaitFor(rnwDb);

builder.Build().Run();
