public interface IRateLimiterService
{
    bool CanSend(string senderNumber);
    bool CanSendGlobal();
    int GetGlobalUsagePercentage();
    int GetSenderUsagePercentage(string senderNumber);
} 