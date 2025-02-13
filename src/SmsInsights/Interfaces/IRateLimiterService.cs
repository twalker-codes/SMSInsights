public interface IRateLimiterService
{
    bool CanSend(string senderPhoneNumber);
    bool CanSendGlobal();
    int GetGlobalUsagePercentage();
    int GetSenderUsagePercentage(string senderNumber);
    int GetCountForKey(string key);
    int GetMaxMessagesPerSender();
    int GetMaxMessagesGlobal();
    void CleanupInactiveSenders(TimeSpan inactivityThreshold);
} 