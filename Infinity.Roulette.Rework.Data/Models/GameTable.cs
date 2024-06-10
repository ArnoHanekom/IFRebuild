using Infinity.Roulette.Rework.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infinity.Roulette.Rework.Data.Models;

public class GameTable
{
    private readonly object _lock = new();    
    public Guid Id { get; init; }
    public int TableNr { get; init; }
    public Game Game { get; set; }
    public bool RunSpinfile { get; set; } = false;
    public bool LimitMatch
    { 
        get
        {
            if (HasRowLimit && !HasGSLimit && !HasCountLimit) return RowMatch;
            if (HasRowLimit && HasGSLimit && !HasCountLimit) return (RowMatch && GSMatch);
            if (HasRowLimit && HasGSLimit && HasCountLimit) return (RowMatch && GSMatch && CountMatch);

            if (!HasRowLimit && HasGSLimit && !HasCountLimit) return GSMatch;
            if (!HasRowLimit && HasGSLimit && HasCountLimit) return (GSMatch && CountMatch);

            if (!HasRowLimit && !HasGSLimit && HasCountLimit) return CountMatch;
            if (HasRowLimit && !HasGSLimit && HasCountLimit) return (CountMatch && RowMatch);

            return false;
        }
    }
    public bool Completed { get; set; } = false;

    private BoardLayout _gameBoard
    {
        get
        {
            return Game.Roulette.BoardLayouts[0];
        }
    }
    public int Spins
    {
        get
        {
            return Game.Roulette.Spins;
        }
    }
    public int Rows
    {
        get
        {
            return _gameBoard.Matrix.Rows.Count;
        }
    }
    public int GS
    {
        get
        {
            return _gameBoard.Matrix.GapSize;
        }
    }
    public int MaxGS
    {
        get
        {
            return _gameBoard.Matrix.MaxGapSize;
        }
    }
    public int Counts
    {
        get
        {
            return _gameBoard.Columns.SelectMany(col => col.Numbers).Count(num => num.Codes.Count <= 1);
        }
    }
    public int FirstRowWin
    {
        get
        {
            return _gameBoard.CodeWins[0];
        }
    }
    private KeyValuePair<int, int> R1Row
    {
        get
        {   
            return _gameBoard.CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault();
        }
    }    
    public int? R1WLimit { get; set; }
    public bool R1WMatch
    {
        get
        {
            return R1WLimit.HasValue && FirstRowWin >= R1WLimit.Value && _r1wHighest(R1Row);
        }
    }
    public GameTable()
    {
        Id = Guid.NewGuid();
        Game = new();
    }
    public void SetGame(Game game)
    {
        lock (_lock) 
        {
            Game = game;
        }
    }

    public int HighestColumnWin
    {
        get
        {
            return _gameBoard.Columns.OrderByDescending(c => c.ColumnWins).First().ColumnWins;
        }
    }

    public int ColumnWinZeroCount
    {
        get
        {
            return _gameBoard.Columns.Count(col => col.ColumnWins == 0);
        }
    }

    public int ColumnWinOneCount
    {
        get
        {
            return _gameBoard.Columns.Count(col => col.ColumnWins == 1);
        }
    }

    public int? TWLimit { get; set; }
    public bool TWMatch
    {
        get
        {
            return TWLimit.HasValue && HighestColumnWin >= TWLimit.Value;
        }
    }
    public int? RowLimit { get; set; }
    public int? GSLimit { get; set; }
    public int? CountLimit { get; set; }

    public bool HasRowLimit => RowLimit.HasValue;
    public bool HasGSLimit => GSLimit.HasValue;
    public bool HasCountLimit => CountLimit.HasValue;

    public bool RowMatch
    {
        get
        {
            return HasRowLimit ? Rows >= RowLimit!.Value : false;
        }
    }
    public bool GSMatch
    {
        get
        {
            return HasGSLimit ? GS >= GSLimit!.Value : false;
        }
    }
    public bool CountMatch
    {
        get
        {
            return HasCountLimit ? Counts >= CountLimit!.Value : false;
        }
    }

    public int BoardResets { get; set; } = 0;

    private Func<KeyValuePair<int, int>, bool> _r1wHighest = r1w => r1w.Key == 0;
}