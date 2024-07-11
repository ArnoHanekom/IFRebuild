using Infinity.Services.Interfaces;

namespace Infinity.Roulette.ViewModels;

public class SplashScreenViewModel(ILicenseService licenseService) : ViewModelBase
{
    public async Task<bool> IsValidLicenseAsync()
    {
        if (await licenseService.HasLicenseAsync())
        {
            await licenseService.LoadLicenseAsync();
            return await licenseService.IsActiveLicenseAsync();
        }
        return false;
    }
}