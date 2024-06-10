using Infinity.Roulette.Rework.Data.Models;

namespace Infinity.Roulette.Rework.Services;

public interface IPlayService
{
    public Task SetPrepareReporterAsync(IProgress<double> reporter, CancellationToken ct = default);
    public Task SetProgressReporterAsync(IProgress<double> reporter, CancellationToken ct = default);
    public Task PlayAsync(int autoplayCount, int tableCount, int spinCount, SpinType spinType, CancellationToken ct = default);
    public Task PrepareAutoplaysAsync(int autoplayCount, int tableCount, CancellationToken ct = default);
    public Task WriteHistoryAsync(CancellationToken ct = default);
    public Task<SearchResult[]> GetResultsAsync(CancellationToken ct = default);
    public Task EndGamePhaseAsync(Guid tableId, Guid gameId, SpinType spinType, CancellationToken ct = default);

    public Task RunAutoplaysTablesSpinfileAsync(List<int> spinNumbers, SpinType spinType, CancellationToken ct = default);
    public Task ResetSearchAsync(CancellationToken ct = default);
}