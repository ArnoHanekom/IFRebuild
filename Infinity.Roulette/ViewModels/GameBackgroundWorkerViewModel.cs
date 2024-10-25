using Infinity.Data.Models;
using Infinity.Roulette.Workers;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace Infinity.Roulette.ViewModels;

public class GameBackgroundWorkerViewModel : ViewModelBase
{
    private TaskCompletionSource<List<Table>> _tcs { get; set; } = new();

    private ConcurrentBag<Table> results = new();

    private AutoplayWorker _autoplayWorker { get; set; } = null!;
    private TableWorker _tableWorker { get; set; } = null!;
    public GameBackgroundWorkerViewModel()
    {
        _tableWorker = new();
        _tableWorker.DoWork += _tableWorker_DoWork;
        _tableWorker.ProgressChanged += _tableWorker_ProgressChanged;
        _tableWorker.RunWorkerCompleted += _tableWorker_RunWorkerCompleted;

        _autoplayWorker = new();
        _autoplayWorker.DoWork += _autoplayWorker_DoWork;
        _autoplayWorker.ProgressChanged += _autoplayWorker_ProgressChanged;
        _autoplayWorker.RunWorkerCompleted += _autoplayWorker_RunWorkerCompleted;


    }

    public Task<List<Table>> PlayTables(IProgress<string> tableProgress)
    {
        _tcs.SetResult([.. results]);
        return _tcs.Task;
    }

    private void _tableWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void _tableWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void _tableWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        int tables = (int)(e.Argument ?? 10);
    }

    private void _autoplayWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
    }

    private void _autoplayWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
    }

    private void _autoplayWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        int autoplays = (int)(e.Argument ?? 10);

    }


}