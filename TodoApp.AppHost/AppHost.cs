
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
      .AddPostgres("db")
      .WithContainerName("todo-db");

if (builder.Configuration.GetValue("UseVolumes", true)) {
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
