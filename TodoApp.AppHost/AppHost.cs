
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var adminUser = builder.AddParameter("AdminUser");
var adminPassword = builder.AddParameter("AdminPassword");
var containerName = builder.AddParameter("DBContainerName", "todo-db");
var useVolumes = builder.Configuration.GetValue("UseVolumes", true);

var postgres = builder
      .AddPostgres("db")
      .WithContainerName(await containerName.Resource.GetValueAsync(CancellationToken.None) ?? "todo-db");

if (useVolumes) {
  postgres.WithDataVolume("appdata");
}


var db = postgres
    .AddDatabase("appdata");

var migrations = builder
      .AddProject<Projects.TodoApp_MigrationService>("migrations")
      .WithReference(db)
      .WaitFor(db);

var apiService = builder
    .AddProject<Projects.TodoApp_ApiService>("apiservice")
    .WithEnvironment("AdminUser", adminUser)
    .WithEnvironment("AdminPassword", adminPassword)
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WithReference(migrations)
    .WaitForCompletion(migrations)
    .WaitFor(db);

builder.AddProject<Projects.TodoApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
