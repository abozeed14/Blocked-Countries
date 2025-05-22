using BlockedCountriesApi.BackgroundServices;
using BlockedCountriesApi.Core.Interfaces;
using BlockedCountriesApi.Core.Services;
using BlockedCountriesApi.Models;
using BlockedCountriesApi.Services.Implementaion;
using System.Runtime;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.Configure<GeoSettings>(builder.Configuration.GetSection("GeoSettings"));

builder.Services.AddSingleton<IGeoService, GeoService>();
builder.Services.AddSingleton<IBlockService, BlockService>();
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddHostedService<TemporalBlockCleanupService>();

var app = builder.Build();


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
