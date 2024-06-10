// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.ResultsViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Data.Models;
using Infinity.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Infinity.Roulette.ViewModels
{
  public class ResultsViewModel : ViewModelBase
  {
    private readonly ISearchService _searchService;

    public ResultsViewModel(ISearchService searchService)
    {
      _searchService = searchService;
      SearchResults = Task.Run(async () => await _searchService.GetResults()).Result;
    }

    private IEnumerable<Table> _searchResults { get; set; }

    public IEnumerable<Table> SearchResults
    {
      get => _searchResults;
      set
      {
        if (_searchResults != value)
        {
          _searchResults = value;
          ObsSearchResults = new ObservableCollection<Table>(_searchResults);
        }
        OnPropertyChanged(nameof (SearchResults));
      }
    }

    private List<Table> _resultList { get; set; }

    public List<Table> ResultList
    {
      get => _resultList;
      set
      {
        if (_resultList != value)
          _resultList = value;
        OnPropertyChanged(nameof (ResultList));
      }
    }

    private double _spinPercentage { get; set; }

    public double SpinPercentage
    {
      get => _spinPercentage;
      set
      {
        if (_spinPercentage != value)
          _spinPercentage = value;
        OnPropertyChanged(nameof (SpinPercentage));
      }
    }

    private ObservableCollection<Table> _obsSearchResults { get; set; }

    public ObservableCollection<Table> ObsSearchResults
    {
      get => _obsSearchResults;
      set
      {
        if (_obsSearchResults != value)
          _obsSearchResults = value;
        OnPropertyChanged(nameof (ObsSearchResults));
      }
    }

    public async Task GetLatestResults() => SearchResults = await _searchService.GetResults();

    public async Task GetLatestTopResults() => ResultList = (await _searchService.GetResults()).Take(10).ToList();
  }
}
