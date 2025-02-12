namespace SmsInsights.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents an SMS message request.
/// </summary>
public class SmsRequest
{
    [Required]
    public string? SenderPhoneNumber { get; set; }
    
    [Required]
    public string? ReceiverPhoneNumber { get; set; }
    
    [Required]
    public string? Message { get; set; }
}
