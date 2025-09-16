using MusicMan.WebApi.Endpoints.Collection;
using MusicMan.WebApi.Endpoints.WeatherForecast;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// REPR: map endpoints defined with co-located Request/Endpoint/Response types
app.MapWeatherForecast();
app.MapAddByBarcode();

app.Run();

// Records moved into their respective endpoint files (REPR)
