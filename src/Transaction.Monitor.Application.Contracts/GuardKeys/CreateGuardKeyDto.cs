using System.ComponentModel.DataAnnotations;

namespace Transaction.Monitor.GuardKeys;

public class CreateGuardKeyDto
{
    [Required]
    public string AppId { get; set; } = string.Empty;
}