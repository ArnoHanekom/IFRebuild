using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Infinity.Roulette.Controls;

public partial class SearchProgress : UserControl, INotifyPropertyChanged
{
    private double _progressSearch { get; set; }
    public double ProgressSearch
    {
        get => _progressSearch;
        set
        {
            _progressSearch = value;
            OnPropertyChanged(nameof(ProgressSearch));
        }
    }
    public SearchProgress()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty SetPercentageProperty =
        DependencyProperty.Register("Percentage", typeof(double), typeof(SearchProgress),
        new PropertyMetadata(default(double), new PropertyChangedCallback(OnPercentageChanged)));

    private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        SearchProgress? progress = d as SearchProgress;
        progress!.OnPercentageChanged(e);
    }

    public double Percentage
    {
        get { return (double)GetValue(SetPercentageProperty); }
        set { SetValue(SetPercentageProperty, value); }
    }

    private void OnPercentageChanged(DependencyPropertyChangedEventArgs e)
    {
        ProgressSearch = double.TryParse(e.NewValue.ToString(), out double percValue) ? percValue : 0.0;
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