using SmsInsights.Interfaces;
using SmsInsights.Models;
using SmsInsights.Options;

namespace SmsInsights.Services;

public class MessageService : IMessageService
{
    private readonly IRateLimiterService _rateLimiter;

    public MessageService(IRateLimiterService rateLimiter)
    {
        _rateLimiter = rateLimiter;
    }

    public bool CanSendMessage(SmsRequest request)
    {
        if (string.IsNullOrEmpty(request.SenderPhoneNumber))
            return false;
        
        return _rateLimiter.CanSendGlobal() && _rateLimiter.CanSend(request.SenderPhoneNumber);
    }

    public MessageResponse SendMessage(SmsRequest request)
    {
        if (!CanSendMessage(request))
        {
            return MessageResponse.FailureResponse(ApiResponseMessages.RateLimitExceeded);
        }

        return MessageResponse.SuccessResponse(ApiResponseMessages.MessageSentSuccess);
    }
}
