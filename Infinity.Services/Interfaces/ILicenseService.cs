namespace Infinity.Services.Interfaces;

public interface ILicenseService
{
    Task<bool> HasLicenseAsync();
    Task LoadLicenseAsync();
    Task<bool> IsActiveLicenseAsync();
}