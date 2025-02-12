namespace SmsInsights.Interfaces;

public interface IRateLimiter
{
    bool CanSend(string senderPhoneNumber);
    bool CanSendGlobal();
}
