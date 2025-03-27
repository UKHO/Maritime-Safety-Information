var builder = DistributedApplication.CreateBuilder(args);

var rnwDb = builder.AddSqlServer("sql")
    .WithDataVolume()
    .AddDatabase("RadioNavigationalWarningsDb");

builder.AddProject<Projects.UKHO_MaritimeSafetyInformation_Web>("ukho-msi-web")
    .WithReference(rnwDb)
    .WaitFor(rnwDb);

builder.Build().Run();
