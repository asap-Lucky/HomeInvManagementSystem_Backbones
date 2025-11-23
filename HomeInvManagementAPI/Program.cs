using HomeInvManagementAPI.Interfaces;
using HomeInvManagementAPI.Services;
using HomeInvManagementAPI.Services.OpenFoodFacts;
using HomeInvManagementAPI.Interfaces.OpenFoodFacts;
using System.Reflection;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Service injection
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddHttpClient<IProductAggregatorService, ProductAggregatorService>();
    builder.Services.AddHttpClient<IOpenFoodFactsService, OpenFoodFactsService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (ReflectionTypeLoadException ex)
{
    foreach (var loaderException in ex.LoaderExceptions)
    {
        Console.WriteLine(loaderException.Message);
    }
    throw;
}
catch (Exception ex)
{
    
}
