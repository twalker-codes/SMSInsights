using SmsInsights.Interfaces;
using SmsInsights.Services;
using SmsInsights.Data;
using SmsInsights.Options;
using StackExchange.Redis;
using Serilog;
using SmsInsights.Models;
using SmsInsights.Api;

var builder = WebApplication.CreateBuilder(args);

// Load configuration settings
var appSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();
if (appSettings == null)
{
    appSettings = new ApplicationSettings();
    Log.Warning("Application settings not found. Using default values.");
}
builder.Services.AddSingleton(appSettings);

// Use the ReactClient settings from the ApplicationSettings
var reactClientOrigin = appSettings.ReactClient.Origin;

// Add CORS configuration to allow requests from the React client.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins(reactClientOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure Redis and other services as usual...
var redisConnection = ConnectionMultiplexer.Connect(appSettings.Redis.ConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IRateLimiterService>(sp => new RateLimiterService(
    sp.GetRequiredService<IRedisService>(),
    appSettings.RateLimits.MaxMessagesPerSenderPerSec,
    appSettings.RateLimits.MaxMessagesGlobalPerSec
));
builder.Services.AddSingleton<IMessageService, MessageService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReactClient");

// Register messaging endpoints.
app.RegisterMessagingEndpoints();

app.Run();
Log.CloseAndFlush();