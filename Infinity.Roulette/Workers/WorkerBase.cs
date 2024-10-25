using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infinity.Roulette.Workers;

public abstract class WorkerBase : BackgroundWorker, INotifyPropertyChanged
{
    protected WorkerBase()
    {
        WorkerSupportsCancellation = true;
        WorkerReportsProgress = true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler propertyChanged = PropertyChanged!;
        if (propertyChanged == null)
            return;
        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}