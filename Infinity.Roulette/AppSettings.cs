using Infinity.Data.Models;
using Infinity.Roulette.Rework.Data.Models;
using Infinity.Roulette.Rework.Services;
using Infinity.Services.Interfaces;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.IO;

namespace Infinity.Roulette
{
    public class AppSettings
    {
        private readonly IReworkSettingService _reworkSettingService;
        private readonly ISettingService _settingService;
        private readonly ITableSettingService _tableSettingService;

        public AppSettings(ISettingService settingService, ITableSettingService tableSettingService, IReworkSettingService reworkSettingService)
        {
            _reworkSettingService = reworkSettingService;
            _settingService = settingService;
            _tableSettingService = tableSettingService;
        }

        public void LoadSettings()
        {
            FileInfo settingsFile = new("dashboard-settings.json");
            if (settingsFile.Exists)
            {
                DashSetting dashSetting = JsonConvert.DeserializeObject<DashSetting>(new StreamReader(settingsFile.Open(FileMode.Open)).ReadToEnd())!;
                if (dashSetting is not null)
                    _reworkSettingService.IngestSavedSettings(dashSetting);
            }
            //FileInfo fileInfo1 = new FileInfo("settings.json");
            //if (fileInfo1.Exists)
            //{
            //    Setting setting = JsonConvert.DeserializeObject<Setting>(new StreamReader(fileInfo1.Open(FileMode.Open)).ReadToEnd())!;
            //    if (setting != null)
            //        _settingService.Save(setting);
            //}
            //FileInfo fileInfo2 = new FileInfo("table-settings.json");
            //if (!fileInfo2.Exists)
            //    return;
            //TableSetting setting1 = JsonConvert.DeserializeObject<TableSetting>(new StreamReader(fileInfo2.Open(FileMode.Open)).ReadToEnd())!;
            //if (setting1 == null)
            //    return;
            //_tableSettingService.Save(setting1);
        }
    }
}
