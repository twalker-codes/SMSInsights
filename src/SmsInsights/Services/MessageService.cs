using SmsInsights.Interfaces;
using SmsInsights.Models;
using SmsInsights.Options;

namespace SmsInsights.Services;

public class MessageService : IMessageService
{
    private readonly IRateLimiter _rateLimiter;

    public MessageService(IRateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter;
    }

    public bool CanSendMessage(SmsRequest request)
    {
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
