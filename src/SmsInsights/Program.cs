using SmsInsights.Interfaces;
using SmsInsights.Services;
using SmsInsights.Data;
using SmsInsights.Options;
using StackExchange.Redis;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Load configuration settings
var appSettings = new ApplicationSettings();
builder.Configuration.GetSection("ApplicationSettings").Bind(appSettings);
builder.Services.AddSingleton(appSettings);

// Configure Redis
var redisConnection = ConnectionMultiplexer.Connect(appSettings.Redis.ConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);

// Register Redis service and rate limiter service
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddSingleton<IRateLimiter>(sp => new RateLimiterService(
    sp.GetRequiredService<IRedisService>(), 
    appSettings.RateLimits.MaxMessagesPerSenderPerSec, 
    appSettings.RateLimits.MaxMessagesGlobalPerSec
));

builder.Services.AddSingleton<IMessageService, MessageService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }

// Register API Endpoints
app.Run();
Log.CloseAndFlush();
