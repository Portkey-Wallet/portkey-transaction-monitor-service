using System.ComponentModel.DataAnnotations;

namespace Transaction.Monitor.AddressConfigs;

public class AddressConfigListDto
{
    [Required]
    public string AppId { get; set; } = string.Empty;

}