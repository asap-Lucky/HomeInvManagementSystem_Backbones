using Application.Commands;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Interfaces.Repositories;
using Application.Queries;
using HomeInvManagementAPI.DI;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Service injection
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Commands
builder.Services.AddTransient<IInventoryCommand, InventoryCommand>();
builder.Services.AddTransient<IImageCommand, ImageCommand>();

// Queries
builder.Services.AddTransient<IInventoryQuery, InventoryQuery>();
builder.Services.AddTransient<IImageQuery, ImageQuery>();
builder.Services.AddTransient<IOpenFoodFactsQuery, OpenFoodFactsQuery>();

var homeInvConnectionString = builder.Configuration["ConnectionStrings:HomeInvConnectionString"];
var homeInvServerVersion = new MySqlServerVersion(new Version(8, 0, 44));

builder.Services.AddDbContext<HomeinvsystemContext>(optionsBuilder => optionsBuilder
                .UseMySql(homeInvConnectionString, homeInvServerVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
    
builder.Services.AddInfrastructure(options =>
{
    options.OFFApiUrl = builder.Configuration["ExternalApis:OpenFoodFactsUrl"];
    options.OFFAuthToken = builder.Configuration["Credentials:OpenFoodFactsAuthCredentials"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.)
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "api");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();