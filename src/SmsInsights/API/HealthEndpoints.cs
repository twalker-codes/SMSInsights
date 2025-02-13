using Microsoft.AspNetCore.Mvc;
using SmsInsights.Interfaces;

namespace SmsInsights.Api;

public static class HealthEndpoints
{
    public static void RegisterHealthEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", CheckHealth)
            .WithName("HealthCheck")
            .WithTags("Health")
            .WithOpenApi();
    }

    private static async Task<IResult> CheckHealth(IRedisService redisService)
    {
        try
        {
            var isHealthy = await Task.Run(() => {
                try
                {
                    redisService.GetCount("health");
                    return true;
                }
                catch
                {
                    return false;
                }
            });

            return isHealthy 
                ? Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow })
                : Results.StatusCode(503);
        }
        catch
        {
            return Results.StatusCode(500);
        }
    }
} 