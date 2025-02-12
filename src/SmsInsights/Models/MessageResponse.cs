namespace SmsInsights.Models;

/// <summary>
/// Represents the response of a message operation.
/// </summary>
public class MessageResponse
{
    public bool Success { get; }
    public string Message { get; }

    private MessageResponse(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public static MessageResponse SuccessResponse(string message) =>
        new MessageResponse(true, message);

    public static MessageResponse FailureResponse(string message) =>
        new MessageResponse(false, message);
}
