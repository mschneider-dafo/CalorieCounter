var builder = DistributedApplication.CreateBuilder(args);

var sqlite = builder.AddSqlite("sqlite");

var jwtKey = builder.AddParameter("jwt-key", secret:true);

var apiService = builder.AddProject<Projects.CalorieCounter_ApiService>("apiservice")
    .WithReference(sqlite)
    .WithEnvironment("Jwt__Key",jwtKey)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
