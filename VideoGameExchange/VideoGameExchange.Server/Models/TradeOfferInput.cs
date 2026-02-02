using System.ComponentModel.DataAnnotations;

namespace VideoGameExchange.Server.Models;

public class TradeOfferInput
{
    [Required]
    public string OfferorUserId { get; set; } = string.Empty;
    
    [Required]
    public string OffereeUserId { get; set; } = string.Empty;
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one game must be offered")]
    public List<string> OfferedGameIds { get; set; } = new();
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one game must be requested")]
    public List<string> RequestedGameIds { get; set; } = new();
    
    public string? Message { get; set; }
}
