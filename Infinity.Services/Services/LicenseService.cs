using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace Infinity.Services.Services;

public class LicenseService : ILicenseService
{
    public const string licenseFilename = "InfinityRoulette.lic";
    public const string activateFilename = "InfinityRoulette-Act.lic";
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
        if (activeLicense is null) return false;

        TimeSpan difference = current.Date - activeLicense.Expire.Date;
        var licenseToBytes = Convert.FromBase64String(_license.AppLicense);
        var licenseDecoded = Encoding.UTF8.GetString(licenseToBytes);
        var activeLicenseToBytes = Convert.FromBase64String(activeLicense.ActiveAppLicense);
        var activeLicenseDecoded = Encoding.UTF8.GetString(activeLicenseToBytes);

        return difference.Days <= 0 && activeLicenseDecoded == licenseDecoded;
    }

    public async Task<bool> GenerateLicenseAsync(int period)
    {
        return await Task.Run(() => GenerateLicense(period));
    }

    private bool GenerateLicense(int period)
    {
        try
        {
            var newAppLicense = Guid.NewGuid().ToString();
            var toBytes = Encoding.UTF8.GetBytes(newAppLicense);
            var base64Encoded = Convert.ToBase64String(toBytes);
            DateTime activeTill = DateTime.Now.AddDays(period);

            License newLicense = new()
            {
                AppLicense = base64Encoded
            };
            ActiveLicense newActiveLicense = new()
            {
                ActiveAppLicense = base64Encoded,
                Expire = activeTill
            };
            FileInfo fileInfo = new(licenseFilename);
            if (fileInfo.Exists)
                fileInfo.Delete();
            using StreamWriter licenseWriter = new(fileInfo.Open(FileMode.OpenOrCreate));
            licenseWriter.Write(JsonConvert.SerializeObject(newLicense));

            fileInfo = new(activateFilename);
            if (fileInfo.Exists)
                fileInfo.Delete();
            using StreamWriter activeLicenseWriter = new(fileInfo.Open(FileMode.OpenOrCreate));
            activeLicenseWriter.Write(JsonConvert.SerializeObject(newActiveLicense));

            return true;
        }
        catch
        {
            return false;
        }
    }
}