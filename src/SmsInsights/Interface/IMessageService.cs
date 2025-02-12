using SmsInsights.Models;

namespace SmsInsights.Interfaces;

public interface IMessageService
{
    bool CanSendMessage(SmsRequest request);
    MessageResponse SendMessage(SmsRequest request);
}
