var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres")
                      .WithDataVolume(isReadOnly: false).WithPgWeb();

var postgresdb = postgres.AddDatabase("postgresdb");

var migrationService = builder.AddProject<Projects.PSPOS_MigrationService>("migration")
    .WithReference(postgresdb);

var apiService = builder.AddProject<Projects.PSPOS_ApiService>("apiservice").WithReference(postgresdb);

builder.AddProject<Projects.PSPOS_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();