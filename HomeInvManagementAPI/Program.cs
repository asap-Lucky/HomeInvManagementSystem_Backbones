using Application.Commands;
using Application.Interfaces.Commands;
using Application.Interfaces.Queries;
using Application.Interfaces.Repositories;
using Application.Queries;
using HomeInvManagementAPI.DI;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Service injection
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddTransient<IInventoryCommand, InventoryCommand>();
builder.Services.AddTransient<IInventoryQuery, InventoryQuery>();
builder.Services.AddTransient<IOpenFoodFactsQuery, OpenFoodFactsQuery>();
builder.Services.AddTransient<IInventoryManagementRepository, InventoryManagementRepository>();

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