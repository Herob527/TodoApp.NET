using Microsoft.EntityFrameworkCore;
using TodoApp.ApiService;

var builder = WebApplication.CreateBuilder(args);


// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddDbContext<DbContext>();

builder.Services.AddAuthentication().AddBearerToken();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.MapDefaultEndpoints();

app.MapTodoEndpoints();

app.Run();
