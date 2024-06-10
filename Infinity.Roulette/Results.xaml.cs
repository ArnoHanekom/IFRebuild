// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.Results
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Unity;


#nullable enable
namespace Infinity.Roulette
{
    public partial class Results : Window
    {
        private ResultsViewModel resultsVM = Container.container.Resolve<ResultsViewModel>();

        public Results()
        {
            DataContext = resultsVM;
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
        }

        public async Task<bool> ReloadResultsGrid()
        {
            await resultsVM.GetLatestResults();
            return true;
        }

        public void LoadResultsGrid() => Task.Run(async () => await resultsVM.GetLatestResults());

        public async Task UpdateSpinProgress(double progress)
        {
            double num = await Task.Run(() => resultsVM.SpinPercentage = progress);
        }

        private async void btnResults_Click(object sender, RoutedEventArgs e) => await resultsVM.GetLatestTopResults();

        public void UpdateResultsSpinProgress(double progress) => resultsVM.SpinPercentage = progress;

    }
}
