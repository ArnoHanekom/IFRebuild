using Infinity.Data.Constants;
using Infinity.Roulette.Rework.Data.Models;

namespace Infinity.Roulette.Rework.Services.Impl;

public class ReworkSettingService : IReworkSettingService
{
    private readonly object _lock = new();
    private DashSetting _dashboardSetting { get; set; } = new();

    public async Task CreateNewDashboardSettingAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(CreateNewDashboardSetting, ct);
        }
        catch
        {
            CreateNewDashboardSetting();
        }
    }
    private void CreateNewDashboardSetting()
    {
        lock (_lock)
        {
            _dashboardSetting = new();
        }
    }

    public async Task AddTableSettingAsync(ReworkTableSetting tableSetting, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => AddTableSetting(tableSetting), ct);
        }
        catch
        {
            AddTableSetting(tableSetting);
        }
    }
    private void AddTableSetting(ReworkTableSetting tableSetting)
    {
        lock (_lock) 
        { 
            if (!_dashboardSetting.TableSettings.Any(ts => ts.Mode == tableSetting.Mode))
            {
                _dashboardSetting.TableSettings.Add(tableSetting);
            }
            else
            {
                var setting = _dashboardSetting.TableSettings.FirstOrDefault(ts => ts.Mode == tableSetting.Mode);
                if (setting is not null)
                {
                    setting.TablesToPlay = tableSetting.TablesToPlay;
                    setting.Autoplay = tableSetting.Autoplay;
                    setting.Randomize = tableSetting.Randomize;
                    setting.TWLimit = tableSetting.TWLimit;
                    setting.R1WLimit = tableSetting.R1WLimit;
                    setting.RowLimit = tableSetting.RowLimit;
                    setting.CountLimit = tableSetting.CountLimit;
                    setting.GSLimit = tableSetting.GSLimit;
                }
            }
        }
    }

    public async Task<DashSetting> GetDashboardSettingAsync(CancellationToken ct = default)
    {
        try
        {
            return await Task.Run(_getDashboardSetting, ct);
        }
        catch
        {
            return _getDashboardSetting();
        }
    }

    public DashSetting GetDashboardSetting()
    {
        return _getDashboardSetting();
    }

    private DashSetting _getDashboardSetting()
    {
        lock (_lock)
        {
            return _dashboardSetting;
        }
    }

    public async Task ChangeDefaultModeAsync(GameType mode, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => ChangeDefaultMode(mode), ct);
        }
        catch
        {
            ChangeDefaultMode(mode);
        }
    }
    private void ChangeDefaultMode(GameType mode)
    {
        _dashboardSetting.DefaultMode = mode;
    }

    public async Task IngestSavedSettingsAsync(DashSetting setting, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => _ingestSavedSettings(setting), ct);
        }
        catch
        {
            _ingestSavedSettings(setting);
        }
    }
    public void IngestSavedSettings(DashSetting setting)
    {
        _ingestSavedSettings(setting);
    }
    private void _ingestSavedSettings(DashSetting dashSetting)
    {
        lock (_lock)
        {
            _dashboardSetting = dashSetting;
        }
    }

    public async Task<ReworkTableSetting?> GetSettingByGameTypeAsync(GameType selectedType, CancellationToken ct = default)
    {
        return await Task.Run(() => GetSettingByGameType(selectedType), ct);
    }

    public ReworkTableSetting? GetSettingByGameType(GameType selectedType)
    {
        lock (_lock)
        {
            return _dashboardSetting.TableSettings.FirstOrDefault(ts => ts.Mode == selectedType);
        }
    }
}