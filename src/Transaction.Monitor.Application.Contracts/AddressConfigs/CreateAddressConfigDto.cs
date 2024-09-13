using System.ComponentModel.DataAnnotations;

namespace Transaction.Monitor.AddressConfigs;

public class CreateAddressConfigDto
{
    [Required]
    public string AppId { get; set; } = string.Empty;

    [Required]
    public string ToAddress { get; set; } = string.Empty;

    [Required]
    public string CallbackUrl { get; set; } = string.Empty;

}