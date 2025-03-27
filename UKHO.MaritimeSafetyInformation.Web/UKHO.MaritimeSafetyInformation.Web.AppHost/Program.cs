var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.UKHO_MaritimeSafetyInformation_Web>("ukho-maritimesafetyinformation-web");

builder.Build().Run();
