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
            .WithDescription("Sends an SMS message if within rate limits")
            .WithOpenApi(); 
        
        app.MapGet("/api/metrics/global", GetGlobalMetrics)
            .WithTags("Monitoring")
            .WithDescription("Gets global rate limit usage metrics")
            .WithOpenApi();
        
        app.MapGet("/api/metrics/sender/{senderNumber}", GetSenderMetrics)
            .WithTags("Monitoring")
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
        try
        {
            var (isValid, errorMessage) = MessageValidator.ValidateRequest(request);
            if (!isValid)
            {
                return Results.BadRequest(new { error = errorMessage });
            }

            var response = messageService.SendMessage(request);
            if (!response.Success)
            {
                return Results.BadRequest(new 
                { 
                    error = response.Message,
                    rateLimitExceeded = true
                });
            }

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error processing message request");
            return Results.StatusCode(500);
        }
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
