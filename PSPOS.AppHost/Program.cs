var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume(isReadOnly: false).WithPgAdmin();

var postgresdb = postgres.AddDatabase("postgresdb");

var apiService = builder.AddProject<Projects.PSPOS_ApiService>("apiservice").WithReference(postgresdb);

builder.AddProject<Projects.PSPOS_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
