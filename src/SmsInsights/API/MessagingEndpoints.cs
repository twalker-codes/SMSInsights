using Microsoft.AspNetCore.Mvc;
using SmsInsights.Interfaces;
using SmsInsights.Models;
using Serilog;
using SmsInsights.Validators;

namespace SmsInsights.Api;

/// <summary>
/// Provides extension methods for registering messaging-related API endpoints.
/// </summary>
public static class MessagingEndpoints
{
    /// <summary>
    /// Registers all messaging API endpoints.
    /// </summary>
    /// <param name="app">The web application.</param>
    public static void RegisterMessagingEndpoints(this WebApplication app)
    {
        app.MapPost("/api/message/send", SendMessage)
            .WithTags("Messaging")
            .WithOpenApi(); 
        
        app.MapGet("/api/metrics/global", GetGlobalMetrics)
            .WithTags("Monitoring")
            .WithOpenApi();
        
        app.MapGet("/api/metrics/sender/{senderNumber}", GetSenderMetrics)
            .WithTags("Monitoring")
            .WithOpenApi();

        // Update the health check endpoint
        app.MapGet("/api/health", async (HttpContext context) => 
        {
            try
            {
                // Simple Redis check
                var redisService = context.RequestServices.GetRequiredService<IRedisService>();
                var isRedisConnected = await Task.Run(() => {
                    try
                    {
                        redisService.GetCount("health_check");
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (!isRedisConnected)
                {
                    return Results.StatusCode(503);
                }

                return Results.Ok(new { 
                    status = "Healthy", 
                    timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                return Results.StatusCode(500);
            }
        })
        .WithTags("Health")
        .WithOpenApi();
    }

    /// <summary>
    /// Handles sending a message.
    /// </summary>
    /// <param name="request">The message request.</param>
    /// <param name="messageService">The service handling message sending.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    private static IResult SendMessage([FromBody] SmsRequest request, IMessageService messageService)
    {
        var (isValid, errorMessage) = MessageValidator.ValidateRequest(request);
        if (!isValid)
        {
            return Results.BadRequest(MessageResponse.FailureResponse(errorMessage!));
        }

        Log.Information("Processing message request from {Sender}", request.SenderPhoneNumber);
        var response = messageService.SendMessage(request);
        return response.Success ? Results.Ok(response) : Results.BadRequest(response);
    }

    private static IResult GetGlobalMetrics([FromQuery] DateTime from, [FromQuery] DateTime to, IMetricsService metricsService)
    {
        var metrics = metricsService.GetAggregatedGlobalMetrics(from, to);
        return Results.Ok(metrics);
    }

    private static IResult GetSenderMetrics(string senderNumber, [FromQuery] DateTime from, [FromQuery] DateTime to, IMetricsService metricsService)
    {
        var metrics = metricsService.GetAggregatedSenderMetrics(senderNumber, from, to);
        return Results.Ok(metrics);
    }
}
