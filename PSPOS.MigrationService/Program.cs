// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using PSPOS.MigrationService;
using Microsoft.EntityFrameworkCore;
using PSPOS.ApiService.Data;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<ApiDbInitializer>();

builder.AddServiceDefaults();

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgresdb"), npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("PSPOS.MigrationService");
    }));
builder.EnrichNpgsqlDbContext<AppDbContext>(settings =>
    // Disable Aspire default retries if needed
    settings.DisableRetry = true);

var app = builder.Build();

app.Run();