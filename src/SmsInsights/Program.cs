using SmsInsights.Interfaces;
using SmsInsights.Services;
using SmsInsights.Data;
using SmsInsights.Options;
using StackExchange.Redis;
using Serilog;
using SmsInsights.Models;
using SmsInsights.Api;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog using the settings in appsettings.json (and any environment specific override)
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// Bind the ApplicationSettings section and register for DI using IOptions<ApplicationSettings> if needed.
builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("ApplicationSettings"));
var appSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>();

if (appSettings == null)
{
    appSettings = new ApplicationSettings();
    Log.Warning("Application settings not found. Using default values.");
}
builder.Services.AddSingleton(appSettings);

// Optional: log the current environment for verification
Log.Information("Running in {Environment} environment", builder.Environment.EnvironmentName);

// Register CORS using the React client origin from configuration.
var reactClientOrigin = appSettings.ReactClient.Origin;
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // More permissive CORS for development
            policy.SetIsOriginAllowed(origin => true)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            // Strict CORS for production
            policy.WithOrigins(reactClientOrigin)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

// Configure the Redis connection.
var redisConnection = ConnectionMultiplexer.Connect(appSettings.Redis.ConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redisConnection);
builder.Services.AddSingleton<IRedisService, RedisService>();

// Configure the RateLimiterService using values from ApplicationSettings.
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

// Ensure CORS is configured before other middleware
app.UseCors("AllowReactClient");

// Add routing middleware
app.UseRouting();

// Register messaging endpoints
app.RegisterMessagingEndpoints();

app.Run();
Log.CloseAndFlush();