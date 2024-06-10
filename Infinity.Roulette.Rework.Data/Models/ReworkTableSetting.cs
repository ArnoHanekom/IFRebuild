using Infinity.Data.Constants;

namespace Infinity.Roulette.Rework.Data.Models;

public class DashSetting : Setting 
{
    public GameType DefaultMode { get; set; } = GameType.None;
    public List<ReworkTableSetting> TableSettings { get; set; } = new();
    
    /* private readonly object _lock = new();
    public async Task AddTableSettingAsync(TableSetting tableSetting, CancellationToken ct = default)
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
    private void AddTableSetting(TableSetting tableSetting)
    {
        lock (_lock)
        {
            TableSettings.Add(tableSetting);
        }
    }
    */
}

public class ReworkTableSetting : Setting
{
    public required int TablesToPlay { get; set; } = 0;
    public required int Randomize { get; set; } = 0;
    public int? RowLimit { get; set; }
    public int? R1WLimit { get; set; }
    public int? TWLimit { get; set; }
    public int? CountLimit { get; set; }
    public int? GSLimit { get; set; }
    public required int Autoplay { get; set; } = 0;
    public required GameType Mode { get; set; } = GameType.None;
}

public class Setting
{
    public Guid Id { get; init; } = Guid.NewGuid();
}