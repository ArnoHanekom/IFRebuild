using Infinity.Data.Constants;
using Infinity.Roulette.Rework.Data.Models;

namespace Infinity.Roulette.Rework.Services;

public interface IReworkSettingService
{
    public Task CreateNewDashboardSettingAsync(CancellationToken ct = default);
    public Task AddTableSettingAsync(ReworkTableSetting tableSetting, CancellationToken ct = default);
    public Task<DashSetting> GetDashboardSettingAsync(CancellationToken ct = default);
    public Task ChangeDefaultModeAsync(GameType mode, CancellationToken ct = default);
    public Task IngestSavedSettingsAsync(DashSetting setting, CancellationToken ct = default);

    public void IngestSavedSettings(DashSetting setting);
    public DashSetting GetDashboardSetting();

    public Task<ReworkTableSetting?> GetSettingByGameTypeAsync(GameType selectedType, CancellationToken ct = default);
    public ReworkTableSetting? GetSettingByGameType(GameType selectedType);
}