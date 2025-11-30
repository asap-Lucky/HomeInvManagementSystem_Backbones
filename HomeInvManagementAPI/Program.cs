using HomeInvManagementAPI.DI;
using HomeInvManagementAPI.Interfaces.Repositories;
using HomeInvManagementAPI.Interfaces.Services;
using HomeInvManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Service injection
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddTransient<IProductAggregatorService, ProductAggregatorService>();
builder.Services.AddTransient<IOpenFoodFactsService, OpenFoodFactsService>();

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