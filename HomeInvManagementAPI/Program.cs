using HomeInvManagementAPI.Interfaces;
using HomeInvManagementAPI.Services;
using HomeInvManagementAPI.Services.OpenFoodFacts;
using HomeInvManagementAPI.Interfaces.OpenFoodFacts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Transcient
builder.Services.AddTransient<IProductAggregatorService, ProductAggregatorService>();
builder.Services.AddTransient<IOpenFoodFactsService, OpenFoodFactsService>();

// Scoped

// Singleton

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseAuthorization();

app.MapControllers();

app.Run();
