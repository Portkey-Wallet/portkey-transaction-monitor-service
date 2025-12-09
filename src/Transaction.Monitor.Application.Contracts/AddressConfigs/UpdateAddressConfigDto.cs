using System.ComponentModel.DataAnnotations;

namespace Transaction.Monitor.AddressConfigs;

public class UpdateAddressConfigDto
{
    [Required]
    public string AppId { get; set; } = string.Empty;

    [Required]
    public string ToAddress { get; set; } = string.Empty;

    public string ReviseToAddress { get; set; } = string.Empty;

    public string ReviseCallbackUrl { get; set; } = string.Empty;

}