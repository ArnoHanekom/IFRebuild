using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using Newtonsoft.Json;

namespace Infinity.Services.Services;

public class LicenseService : ILicenseService
{
    public const string licenseFilename = "lic.txt";
    public const string activateFilename = "lic-act.txt";
    private License? _license { get; set; } = null;

    public async Task<bool> HasLicenseAsync()
    {
        return await Task.Run(hasLicenseFile);
    }
    private bool hasLicenseFile()
    {
        FileInfo licenseFile = new(licenseFilename);
        return licenseFile.Exists;
    }

    public async Task LoadLicenseAsync()
    {
        await Task.Run(LoadLicense);
    }

    private void LoadLicense()
    {
        FileInfo licenseFile = new(licenseFilename);
        _license = JsonConvert.DeserializeObject<License>(new StreamReader(licenseFile.Open(FileMode.Open)).ReadToEnd());   
    }

    public async Task<bool> IsActiveLicenseAsync()
    {
        return await Task.Run(isActiveLicense);
    }

    private bool isActiveLicense()
    {
        if (_license is null) return false;
        if (_license.AppLicense == string.Empty) return false;
        DateTime current = DateTime.Now;
        FileInfo activateFile = new(activateFilename);
        ActiveLicense? activeLicense = null;
        if (activateFile.Exists) activeLicense = JsonConvert.DeserializeObject<ActiveLicense>(new StreamReader(activateFile.Open(FileMode.Open)).ReadToEnd());
        activeLicense ??= new() {  ActiveAppLicense = _license.AppLicense, Activated = DateTime.Now };
        TimeSpan difference = current.Date - activeLicense.Activated.Date;
        return difference.Days <= 0 && difference.Days >= 0 && activeLicense.ActiveAppLicense == _license.AppLicense;
    }
}