namespace Infinity.Data.Models;

public class ActiveLicense
{
    public string ActiveAppLicense { get; set; } = string.Empty;
    public DateTime Activated { get; set; } = DateTime.Now;
}