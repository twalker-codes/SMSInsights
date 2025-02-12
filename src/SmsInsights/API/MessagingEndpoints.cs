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

    private static IResult GetGlobalMetrics(IRateLimiterService rateLimiter)
    {
        var metrics = new
        {
            UsagePercentage = rateLimiter.GetGlobalUsagePercentage(),
            Timestamp = DateTime.UtcNow
        };
        return Results.Ok(metrics);
    }

    private static IResult GetSenderMetrics(string senderNumber, IRateLimiterService rateLimiter)
    {
        var metrics = new
        {
            SenderNumber = senderNumber,
            UsagePercentage = rateLimiter.GetSenderUsagePercentage(senderNumber),
            Timestamp = DateTime.UtcNow
        };
        return Results.Ok(metrics);
    }
}
