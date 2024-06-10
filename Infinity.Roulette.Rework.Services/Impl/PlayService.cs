using Infinity.Roulette.Rework.Data.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Infinity.Roulette.Rework.Services.Impl;

public class PlayService : IPlayService
{
    private readonly object _lock = new();
    private List<History> _history { get; set; } = new();
    private List<Autoplay> _autoplayList { get; set; } = new();
    private List<SearchResult> _searchResults { get; set; } = new();
    private List<GameTable> _tablesForSpinfile { get; set; } = new();
    private IProgress<double> _progressReporter { get; set; } = default!;
    private IProgress<double> _prepareReporter { get; set; } = default!;

    private int spinCounter = 0;
    private int spinTotal = 0;
    private int prepped = 0;
    private int totalToPrep = 0;

    private void ResetSearch()
    {
        lock (_lock)
        {
            _history = new();
            _autoplayList = new();
            _searchResults = new();
            _progressReporter = default!;
            _prepareReporter = default!;
        }
    }

    public async Task ResetSearchAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(ResetSearch, ct);
        }
        catch
        {
            ResetSearch();
        }
    }

    public async Task UpdateProgressReporterAsync(double progress, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => UpdateProgressReporter(progress), ct);
        }
        catch
        {
            UpdateProgressReporter(progress);
        }
    }
    private void UpdateProgressReporter(double progress)
    {
        if (_progressReporter is not null)
            _progressReporter.Report(progress);
    }

    public async Task UpdatePrepareReporterAsync(double progress, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => UpdatePrepareReporter(progress), ct);
        }
        catch
        {
            UpdatePrepareReporter(progress);
        }
    }
    private void UpdatePrepareReporter(double progress)
    {
        _prepareReporter.Report(progress);
    }

    public async Task SetProgressReporterAsync(IProgress<double> reporter, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => SetProgressReporter(reporter), ct);
        }
        catch
        {
            SetProgressReporter(reporter);
        }
    }
    private void SetProgressReporter(IProgress<double> reporter)
    {
        _progressReporter = reporter;
    }

    public async Task SetPrepareReporterAsync(IProgress<double> reporter, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => SetPrepareReporter(reporter), ct);
        }
        catch
        {
            SetPrepareReporter(reporter);
        }
    }
    private void SetPrepareReporter(IProgress<double> reporter)
    {
        _prepareReporter = reporter;
    }
    
    public async Task PrepareAutoplaysAsync(int autoplayCount, int tableCount, CancellationToken ct = default)
    {
        _autoplayList.Clear();
        //Prep start
        prepped = 0;
        totalToPrep = autoplayCount * tableCount;
        await UpdatePrepareReporterAsync(0, ct).ConfigureAwait(false);
        for (int a = 0; a < autoplayCount; a++)
        {
            if (ct.IsCancellationRequested) break;
            
            var autoplay = await GetNewAutoplayAsync(ct).ConfigureAwait(false);
            autoplay.SetTables(await PrepareAutoplayTablesAsync(a + 1, tableCount, ct).ConfigureAwait(false));
            lock (_lock)
            {
                _autoplayList.Add(autoplay);
            }            
        }
        //Prep completed
        await UpdatePrepareReporterAsync(100.00, ct).ConfigureAwait(false);
    }

    public async Task<List<GameTable>> PrepareAutoplayTablesAsync(int autoplayNr, int tableCount, CancellationToken ct = default)
    {
        //prep update
        //await UpdateProgressReporterAsync(0, ct).ConfigureAwait(false);
        List<GameTable> tables = new();
        var prefix = (autoplayNr - 1) * 10;
        for (int t = 0; t < tableCount; t++)
        {
            if (ct.IsCancellationRequested) break;

            tables.Add(await GetNewGameTableAsync($"{prefix}{t + 1}", ct).ConfigureAwait(false));
            prepped++;
            var prepProg = Math.Round((double)prepped / totalToPrep * 100.00);
            await UpdatePrepareReporterAsync(prepProg, ct).ConfigureAwait(false);
        }
        //prep update
        //await UpdateProgressReporterAsync(100, ct).ConfigureAwait(false);
        return tables;
    }

    public async Task<Autoplay> GetNewAutoplayAsync(CancellationToken ct = default)
    {
        try
        {
            return await Task.Run(GetNewAutoplay, ct);
        }
        catch
        {
            return GetNewAutoplay();
        }        
    }
    private Autoplay GetNewAutoplay()
    {
        return new Autoplay();
    }

    public async Task<GameTable> GetNewGameTableAsync(string tableNr, CancellationToken ct = default)
    {
        try
        {
            return await Task.Run(() => GetNewGameTable(tableNr), ct);
        }
        catch
        {
            return GetNewGameTable(tableNr);
        }
    }
    private GameTable GetNewGameTable(string tableNr)
    {
        return new GameTable() { TableNr = int.Parse(tableNr) };
    }

    public async Task PlayAsync(int autoplayCount, int tableCount, int spinCount, SpinType spinType, CancellationToken ct = default)
    {
        spinCounter = 0;
        spinTotal = autoplayCount * tableCount * spinCount;        
        var autoplayTasks = _autoplayList.Select(a => RunAutoplayTablesAsync(a, spinCount, spinType, ct)).ToArray();
        await Task.WhenAll(autoplayTasks).ConfigureAwait(false);
        await LoadAutoplayResultsAsync(ct).ConfigureAwait(false);
        await UpdateProgressReporterAsync(100, ct).ConfigureAwait(false);
    }

    public async Task RunAutoplayTablesAsync(Autoplay autoplay, int spinCount, SpinType spinType, CancellationToken ct = default)
    {
        var tablesTasks = autoplay.Tables.Select(t => RunTableSpinsAsync(t, spinCount, spinType, ct)).ToArray();
        await Task.WhenAll(tablesTasks).ConfigureAwait(false);
    }    
    
    public async Task RunTableSpinsAsync(GameTable table, int spinCount, SpinType spinType, CancellationToken ct = default)
    {
        for (int spin = 0; spin < spinCount; spin++)
        {
            if (ct.IsCancellationRequested) break;

            var winnumber = await RandomNumberGenerator.NextRandomNumberAsync(ct).ConfigureAwait(false);
            await CaptureSpinAsync(table, spin + 1, winnumber, spinType, ct).ConfigureAwait(false);
            spinCounter++;

            int increments = spinTotal / 1000;
            if (increments <= 0)
            {
                var perc = Math.Round((double)spinCounter / spinTotal * 100.00);
                await UpdateProgressReporterAsync(perc, ct).ConfigureAwait(false);
            }
            else
            {
                if (spinCounter % increments == 0)
                {
                    var perc = Math.Round((double)spinCounter / spinTotal * 100.00);
                    await UpdateProgressReporterAsync(perc, ct).ConfigureAwait(false);
                }
            }
        }

        await AddHistoryAsync(table, ct).ConfigureAwait(false);
        if (ct.IsCancellationRequested)
            await ResetDashboardAsync(ct).ConfigureAwait(false);
    }

    public async Task ResetDashboardAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(ResetDashboard, ct);
        }
        catch
        {
            ResetDashboard();
        }
    }
    private void ResetDashboard()
    {
        lock (_lock)
        {
            _history = new();
            _autoplayList = new();
            _searchResults = new();
            _tablesForSpinfile = new();
            _progressReporter = default!;
            _prepareReporter = default!;
        }
    }

    public async Task CaptureSpinAsync(GameTable table, int spinNr, int winNumber, SpinType spinType, CancellationToken ct = default)
    {
        var phase = await GetTableGameActivePhaseAsync(table.Game, spinType, ct);
        try
        {
            await Task.Run(() => CaptureSpin(phase, table, spinNr, winNumber, spinType), ct);
        }
        catch
        {
            CaptureSpin(phase, table, spinNr, winNumber, spinType);
        }
    }
    private void CaptureSpin(Phase phase, GameTable table, int spinNr, int winNumber, SpinType spinType)
    {
        lock (_lock)
        {
            table.Game.Roulette.CaptureSpin(winNumber);
            bool wasReset = table.Game.Roulette.BoardLayouts[0].AppliedImmovability;

            var spin = new Spin()
            {
                SpinNr = spinNr,
                WinNumber = winNumber,
                Type = spinType
            };

            var numbers = table.Game.Roulette.BoardLayouts[0].Columns.SelectMany(c => c.Numbers);
            var countList = numbers.Where(n => n.Codes.Count <= 1);
            if (countList.Count() > 0)
            {
                foreach (var cn in countList)
                {
                    var countNumber = new CountNumber()
                    {
                        Number = cn.Number,
                        BoardCode = cn.BoardCode,
                        Color = CountColor.None
                    };
                    spin.AddCountNumber(countNumber);
                }
            }

            var previousSpin = phase.Spins.LastOrDefault();

            spin = GetDisplayColor(previousSpin, spin, table.Game);

            if (wasReset)
            {
                table.BoardResets++;
                phase.Ended = true;
                phase.EndedAt = DateTime.Now.Ticks;

                var newPhase = new Phase() { SpinType = spinType };
                newPhase.AddSpin(spin);
                table.Game.AddPhase(newPhase);
            }
            else
            {
                phase.AddSpin(spin);
            }
        }
    }
    private Spin GetDisplayColor(Spin? previousSpin, Spin currentSpin, Game currentGame)
    {
        if (previousSpin is null)
        {
            foreach (var count in currentSpin.CountNumbers)
            {
                count.Color = CountColor.Red;
            }
        }
        else
        {
            var lastResetPhase = currentGame.Phases.Where(p => p.Ended == true).LastOrDefault();
            var fromPrevious = currentSpin.CountNumbers
                .Where(curr => previousSpin.CountNumbers.Any(prev => prev.Number == curr.Number))
                .Select(curr => curr);
            foreach (var prevCount in fromPrevious)
            {
                var current = currentSpin.CountNumbers.FirstOrDefault(curr => curr.Number == prevCount.Number);
                if (current is not null)
                    current.Color = prevCount.Color;
            }

            foreach (var remaining in currentSpin.CountNumbers.Where(curr => curr.Color == CountColor.None))
            {
                if (lastResetPhase is not null)
                {
                    if (currentSpin.Type != SpinType.Random && currentSpin.Type != lastResetPhase.SpinType)
                    {
                        remaining.Color = CountColor.Blue;
                    }
                }
                else
                {
                    if (currentSpin.Type != SpinType.Random && currentSpin.Type != previousSpin.Type)
                    {
                        remaining.Color = CountColor.Blue;
                    }
                }
            }

            foreach (var remaining in currentSpin.CountNumbers.Where(curr => curr.Color == CountColor.None))
            {
                remaining.Color = CountColor.Red;
            }
        }

        return currentSpin;
    }

    private async Task LoadAutoplayResultsAsync(CancellationToken ct = default)
    {
        foreach (var autoplay in _autoplayList)
        {
            await LoadAutoplayTableResultAsync(autoplay, ct);
        }
    }

    private async Task LoadAutoplayTableResultAsync(Autoplay autoplay, CancellationToken ct = default)
    {
        foreach (var table in autoplay.Tables)
        {
            await AddToSearchResultsAsync(table, ct);
        }
    }    

    public async Task<Phase> GetTableGameActivePhaseAsync(Game game, SpinType spinType, CancellationToken ct = default)
    {
        try
        {
            return await Task.Run(() => GetTableGameActivePhase(game, spinType), ct);
        }
        catch
        {
            return GetTableGameActivePhase(game, spinType);
        }
    }
    private Phase GetTableGameActivePhase(Game game, SpinType spinType)
    {
        lock (_lock)
        {
            var currentPhase = game.Phases.LastOrDefault(p => p.Started == true && p.Ended == false);
            if (currentPhase is null)
            {
                currentPhase = new Phase() { SpinType = spinType };
                game.AddPhase(currentPhase);
            }

            return currentPhase;
        }
    }

    public async Task AddHistoryAsync(GameTable table, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => AddHistory(table), ct);
        }
        catch
        {
            AddHistory(table);
        }
    }
    private void AddHistory(GameTable table)
    {
        lock (_lock)
        {
            var tableJson = JsonConvert.SerializeObject(table);
            var history = new History();
            history.AddTableJson(tableJson);
            _history.Add(history);
        }
    }

    public async Task AddToSearchResultsAsync(GameTable table, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => AddToSearchResults(table), ct);
        }
        catch
        {
            AddToSearchResults(table);
        }
    }
    private void AddToSearchResults(GameTable table)
    {
        lock (_lock)
        {
            var newResult = new SearchResult();
            newResult.SetResultTable(table);
            _searchResults.Add(newResult);
        }
    }

    public async Task<SearchResult[]> GetResultsAsync(CancellationToken ct = default)
    {
        try
        { 
            return await Task.Run(GetResults, ct); 
        }
        catch
        { 
            return GetResults(); 
        }
    }
    private SearchResult[] GetResults()
    {
        lock (_lock)
        {
            return _searchResults.ToArray();
        }
    }

    public async Task EndGamePhaseAsync(Guid tableId, Guid gameId, SpinType spinType, CancellationToken ct = default)
    {
        try
        {
            await Task.Run(() => EndGamePhase(tableId, gameId, spinType), ct);
        }
        catch
        {
            EndGamePhase(tableId, gameId, spinType);
        }
    }
    private void EndGamePhase(Guid tableId, Guid gameId, SpinType spinType)
    {
        lock (_lock)
        {
            var tableList = _autoplayList.Where(ap => ap.Tables.Any(t => t.Id == tableId && t.Game.Id == gameId)).SelectMany(ap => ap.Tables);
            foreach (var table in tableList)
            {
                if (table.Id == tableId && table.Game.Id == gameId)
                {
                    var openPhases = table.Game.Phases.Where(p => p.Ended == false);
                    bool phasesClosed = false;
                    foreach (var phase in openPhases)
                    {
                        phase.Ended = true;
                        phasesClosed = true;
                    }

                    if (phasesClosed)
                    {
                        var newPhase = new Phase() { SpinType = spinType };
                        table.Game.AddPhase(newPhase);
                    }
                }
            }
        }
    }

    public async Task WriteHistoryAsync(CancellationToken ct = default)
    {
        try
        {
            await Task.Run(WriteHistory, ct);
        }
        catch
        {
            WriteHistory();
        }
    }
    private void WriteHistory()
    {
        lock (_lock)
        {
            var sorted = _history.OrderBy(h => h.Id);
            var items = sorted.SelectMany(s => s.TablesJson).ToList();
            for (int s = 0; s < items.Count(); s++)
            {
                Debug.WriteLine($"Table {s + 1}");
                Debug.WriteLine($"[{items[s]}]");
                Debug.WriteLine("");
            }

            var deList = items.Select(i => JsonConvert.DeserializeObject<GameTable>(i)).ToList();

            for (int di = 0; di < deList.Count; di++)
            {
                Debug.WriteLine($"Table: {deList[di]?.Id}\tPhases: {deList[di]?.Game.Phases.Count()}");
            }

            var tables = sorted.SelectMany(s => s.TablesJson.Select(t => JsonConvert.DeserializeObject<GameTable>(t)));
            var sortedTables = tables.Where(t => t is not null).OrderBy(t => t?.Id);
            foreach (var table in sortedTables)
            {
                if (table is not null)
                {
                    Debug.WriteLine($"Table: {table.Id}\tGame: {table.Game.Id}");
                    foreach (var phase in table.Game.Phases)
                    {
                        Debug.WriteLine($"\tPhase: {phase.Id}\tStarted: {phase.Started}\tAt: {phase.StartedAt}\tEnded: {phase.Ended}\tAt: {phase.EndedAt}\tSpin Type: {phase.SpinType}");
                        if (phase.Spins.Length > 0)
                        {
                            Debug.WriteLine("\t\tSpins: ");
                            foreach (var spin in phase.Spins)
                            {
                                Debug.WriteLine($"\t\tSpin: {spin.Id}\tSpin #: {spin.SpinNr}\tWinning Number: {spin.WinNumber}{(spin.CountNumbers.Length > 0 ? $"\tCount Numbers: {string.Join("|", spin.CountNumbers.Select(c => $" {c.Number}:{c.BoardCode} "))}" : "")}");

                                if (spin.CountNumbers.Length > 0)
                                {
                                    Debug.WriteLine($"\t\t\tCount Details");

                                    foreach (var count in spin.CountNumbers)
                                    {
                                        Debug.WriteLine($"\t\t\t\tNumber: {count.Number}\tColor: {count.Color}");
                                    }
                                    Debug.WriteLine("");
                                }
                            }
                            Debug.WriteLine("");
                        }
                    }
                }

            }
        }
    }

    public async Task RunAutoplaysTablesSpinfileAsync(List<int> spinNumbers, SpinType spinType, CancellationToken ct = default)
    {
        spinCounter = 0;
        spinTotal = _autoplayList.Count * _autoplayList.Sum(a => a.Tables.Length) * spinNumbers.Count;
        await UpdateProgressReporterAsync(0, ct).ConfigureAwait(false);
        var autoplaySpinfileTasks = _autoplayList.Select(a => RunAutoplaySpinfileTablesAsync(a, spinNumbers, spinType, ct));
        await Task.WhenAll(autoplaySpinfileTasks).ConfigureAwait(false);

        lock (_lock)
        {
            _searchResults.Clear();
        }

        await LoadAutoplayResultsAsync(ct).ConfigureAwait(false);
        await UpdateProgressReporterAsync(100, ct).ConfigureAwait(false);
    }

    public async Task RunAutoplaySpinfileTablesAsync(Autoplay autoplay, List<int> spinNumbers, SpinType spinType, CancellationToken ct = default)
    {
        var tableSpinfileNumbersTasks = autoplay.Tables.Select(t => RunTableSpinfileNumbersAsync(t, spinNumbers, spinType, ct));
        await Task.WhenAll(tableSpinfileNumbersTasks).ConfigureAwait(false);
    }

    public async Task RunTableSpinfileNumbersAsync(GameTable table, List<int> spinNumbers, SpinType spinType, CancellationToken ct = default)
    {
        for (int s = 0; s < spinNumbers.Count; s++)
        {
            var winnumber = spinNumbers[s];
            await CaptureSpinAsync(table, s + 1, winnumber, spinType, ct).ConfigureAwait(false);
            spinCounter++;

            int increments = spinTotal / 1000;

            if (spinCounter % increments == 0)
            {
                var perc = Math.Round((double)spinCounter / spinTotal * 100.00);
                await UpdateProgressReporterAsync(perc, ct).ConfigureAwait(false);
            }
        }

        await AddHistoryAsync(table, ct).ConfigureAwait(false);
    }

}