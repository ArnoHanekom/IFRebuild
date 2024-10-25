namespace Infinity.Roulette.Workers;

public class TableWorker : WorkerBase
{
    private int _progressPercentage { get; set; }
    public int ProgressPercentage
    {
        get => _progressPercentage;
        set
        {
            if (_progressPercentage != value) _progressPercentage = value;
            OnPropertyChanged(nameof(ProgressPercentage));
        }
    }
}