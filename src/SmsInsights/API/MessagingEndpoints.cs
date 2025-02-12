using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OpenApi;
using SmsInsights.Interfaces;
using SmsInsights.Models;
using SmsInsights.Options;
using Swashbuckle.AspNetCore.Annotations;
using Serilog;

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
        app.MapPost("/messages/send", SendMessage)
            .WithTags("Messaging")
            .WithOpenApi(); // Now this method will work
    }

    /// <summary>
    /// Handles sending a message.
    /// </summary>
    /// <param name="request">The message request.</param>
    /// <param name="messageService">The service handling message sending.</param>
    /// <returns>An HTTP response indicating success or failure.</returns>
    private static IResult SendMessage([FromBody] SmsRequest request, IMessageService messageService)
    {
        Log.Information("Processing message request from {Sender}", request.SenderPhoneNumber);
        var response = messageService.SendMessage(request);
        return response.Success ? Results.Ok(response) : Results.BadRequest(response);
    }
}
