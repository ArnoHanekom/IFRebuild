namespace Infinity.Data.Models;

public class ActiveLicense
{
    public string ActiveAppLicense { get; set; } = string.Empty;
    public DateTime Expire { get; set; } = DateTime.Now;
}