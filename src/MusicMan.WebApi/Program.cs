using MusicMan.WebApi.Endpoints.Collection;
using MusicMan.WebApi.Endpoints.WeatherForecast;
using MusicMan.Infrastructure.DependencyInjection;
using MusicMan.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (EF Core with SQLite, health checks)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Apply EF Core migrations in Development automatically
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

// REPR: map endpoints defined with co-located Request/Endpoint/Response types
app.MapWeatherForecast();
app.MapAddByBarcode();

app.Run();

// Records moved into their respective endpoint files (REPR)
