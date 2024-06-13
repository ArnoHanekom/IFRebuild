// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.ViewModels.BettingWindowViewModel
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using Infinity.Data.Constants;
using Infinity.Data.Extensions;
using Infinity.Data.Models;
using Infinity.Engine;
using Infinity.Engine.Services;
using Infinity.Roulette.LayoutModels;
using Infinity.Roulette.Statics;
using Infinity.Services.Interfaces;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


#nullable enable
namespace Infinity.Roulette.ViewModels
{
    public class BettingWindowViewModel : ViewModelBase
    {
        private readonly ICountLabelService _countLabelService;
        private readonly ICountColorService _countColorService;
        private readonly IEngineService _engineService;
        public BettingWindowViewModel(ICountLabelService countLabelService, ICountColorService countColorService, IEngineService engineService)
        {
            _countLabelService = countLabelService;
            _countColorService = countColorService;
            _engineService = engineService;
        }
        private ObservableCollection<BettingWindowLayout> _tableLayout { get; set; } = default!;

        public ObservableCollection<BettingWindowLayout> TableLayout
        {
            get => _tableLayout;
            set
            {
                if (_tableLayout != value)
                    _tableLayout = value;
                OnPropertyChanged(nameof(TableLayout));
            }
        }

        private RouletteGame _game { get; set; } = default!;

        public RouletteGame Game
        {
            get => _game;
            set
            {
                if (_game != value)
                    _game = value;
                OnPropertyChanged(nameof(Game));
            }
        }

        private Style defaultLabel => (Style)Application.Current.FindResource("BetWindowLabel");
        private Style firstRowLabel => (Style)Application.Current.FindResource("BetWindowLabelFirstRow");
        private Style defaultCountLabel => (Style)Application.Current.FindResource("BetWindowCountLabel");
        private Style firstRowCountLabel => (Style)Application.Current.FindResource("BetWindowLabelCountFirstRow");
        private Style rowWinsLabel => (Style)Application.Current.FindResource("BetWindowLabelRowWins");
        private Style tableWinsLabel => (Style)Application.Current.FindResource("BetWindowLabelTableWins");
        private Style rowWinsHighestAndFirstLabel => (Style)Application.Current.FindResource("BetWindowLabelRowWinsHighestAndFirst");
        private Style tableWinsHighestLabel => (Style)Application.Current.FindResource("BetWindowLabelTableWinsHighest");
        private Style defaultTextBlock => (Style)Application.Current.FindResource("BetWindowLabelTextBlock");
        private Style defaultFirstRowTextBlock => (Style)Application.Current.FindResource("BetWindowLabelFirstRowTextBlock");
        private Style defaultCountTextBlock => (Style)Application.Current.FindResource("BetWindowLabelTextBlockRandomSpinCount");
        private Style otherCountTextBlock => (Style)Application.Current.FindResource("BetWindowLabelTextBlockOtherSpinCount");

        private Style GetNumberStyle(BoardNumber boardNumber, int rowIdx, int selectedBoard, CounterNumber[] prevCounts, int currSpinType, Style otherCountStyle)
        {
            if (!boardNumber.IsCount())
            {
                if (rowIdx == selectedBoard)
                    return defaultFirstRowTextBlock;
                else
                    return defaultTextBlock;
            }

            //Debug.WriteLine($"Row: {rowIdx}\tNumber: {boardNumber.Number}\tNumber Codes: {boardNumber.Codes.Count}\tPrev counts: {prevCounts.Count()}\tSpin Type: {currSpinType}");

            var currSpin = currSpinType > 0 ? (GameType)currSpinType : GameType.Random;
            if (currSpin == GameType.Random || currSpin == GameType.Autoplay)
                return defaultCountTextBlock;
            else
            {
                if (prevCounts.Any(pc => pc.Number == boardNumber.Number))
                    return (Style)(prevCounts.First(pc => pc.Number == boardNumber.Number).TextBlockStyle ?? defaultCountTextBlock);
                else
                    return otherCountStyle;
            }
        }

        public void PrepareTableLayout()
        {
            BettingWindowLayout[] collection = new BettingWindowLayout[6];
            int index1 = 0;
            //int index1 = Game.BoardLayouts[0].Matrix.CurrentColumn;
            int selectedBoard = Game.BettingBoard;

            var otherStyle = _countColorService.GetOtherStyle();
            Style countOtherStyle = otherStyle is null ? defaultCountTextBlock : (Style)otherStyle;

            BoardNumber[] spinResults = Game.BoardLayouts[0].Columns.SelectMany(c => c.Numbers).ToArray();
            
            Guid gameTable = Game.TableUniqueId;
            var prevSpinCounts = _countLabelService.GetPreviousSpinCountNumbers(gameTable);
            var spinCounts = _countLabelService.GetCountNumbers(gameTable);
            for (int index2 = 0; index2 < 6; ++index2)
            {
                int code = Game.BoardLayouts[0].Columns[index1].Code;
                Game.SetBetBoardArrangement(index2, code);

                int codeWin = Game.BoardLayouts[0].CodeWins[index2];                
                KeyValuePair<int, int>? nullable = new KeyValuePair<int, int>?(Game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault());
                bool flag1 = false;
                if (index2 == 0 && nullable.HasValue)
                    flag1 = nullable.Value.Key == index2;
                int columnWins = Game.BoardLayouts[0].Columns[index1].ColumnWins;
                bool flag2 = Game.BoardLayouts[0].Columns.HighestColumnWin() == columnWins;
                BoardNumber[] array = ReOrderedColumnNumbers(Game.BoardLayouts[0].Columns[index1]).ToArray();
                Style[] rowNumbersStyles = new Style[6];
                rowNumbersStyles[0] = GetNumberStyle(array[0], index2, selectedBoard, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[1] = GetNumberStyle(array[1], index2, selectedBoard, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[2] = GetNumberStyle(array[2], index2, selectedBoard, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[3] = GetNumberStyle(array[3], index2, selectedBoard, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[4] = GetNumberStyle(array[4], index2, selectedBoard, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[5] = GetNumberStyle(array[5], index2, selectedBoard, prevSpinCounts, Game.SpinType, countOtherStyle);

                TextBlock textBlock1 = new()
                {
                    Text = string.Format("{0}", code),
                    Style = GetDefaultNumberTextBlock(index2, selectedBoard)
                };
                
                TextBlock textBlock2 = new()
                {
                    Text = string.Format("{0}", array[0].Number),
                    Style = rowNumbersStyles[0]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[0].Number, rowNumbersStyles[0]);
                
                TextBlock textBlock3 = new()
                {
                    Text = string.Format("{0}", array[1].Number),
                    Style = rowNumbersStyles[1]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[1].Number, rowNumbersStyles[1]);

                TextBlock textBlock4 = new()
                {
                    Text = string.Format("{0}", array[2].Number),
                    Style = rowNumbersStyles[2]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[2].Number, rowNumbersStyles[2]);

                TextBlock textBlock5 = new()
                {
                    Text = string.Format("{0}", array[3].Number),
                    Style = rowNumbersStyles[3]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[3].Number, rowNumbersStyles[3]);

                TextBlock textBlock6 = new()
                {
                    Text = string.Format("{0}", array[4].Number),
                    Style = rowNumbersStyles[4]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[4].Number, rowNumbersStyles[4]);

                TextBlock textBlock7 = new()
                {
                    Text = string.Format("{0}", array[5].Number),
                    Style = rowNumbersStyles[5]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[5].Number, rowNumbersStyles[5]);

                TextBlock textBlock8 = new()
                {
                    Text = string.Format("{0}", codeWin)
                };
                TextBlock textBlock9 = new()
                {
                    Text = string.Format("{0}", columnWins)
                };
                BettingWindowLayout bettingWindowLayout1 = new();
                Label label1 = new()
                {
                    Content = textBlock1,
                    Style = index2 == selectedBoard ? firstRowLabel : defaultLabel
                };
                bettingWindowLayout1.Code = label1;
                Label label2 = new()
                {
                    Content = textBlock2,
                    Style = GetNumberStyle(index2, selectedBoard, array[0])
                };
                bettingWindowLayout1.Number1 = label2;
                Label label3 = new()
                {
                    Content = textBlock3,
                    Style = GetNumberStyle(index2, selectedBoard, array[1])
                };
                bettingWindowLayout1.Number2 = label3;
                Label label4 = new()
                {
                    Content = textBlock4,
                    Style = GetNumberStyle(index2, selectedBoard, array[2])
                };
                bettingWindowLayout1.Number3 = label4;
                Label label5 = new()
                {
                    Content = textBlock5,
                    Style = GetNumberStyle(index2, selectedBoard, array[3])
                };
                bettingWindowLayout1.Number4 = label5;
                Label label6 = new()
                {
                    Content = textBlock6,
                    Style = GetNumberStyle(index2, selectedBoard, array[4])
                };
                bettingWindowLayout1.Number5 = label6;
                Label label7 = new()
                {
                    Content = textBlock7,
                    Style = GetNumberStyle(index2, selectedBoard, array[5])
                };
                bettingWindowLayout1.Number6 = label7;
                Label label8 = new()
                {
                    Content = textBlock8,
                    Style = flag1 ? rowWinsHighestAndFirstLabel : rowWinsLabel
                };
                bettingWindowLayout1.RowWins = label8;
                Label label9 = new()
                {
                    Content = textBlock9,
                    Style = flag2 ? tableWinsHighestLabel : tableWinsLabel
                };
                bettingWindowLayout1.ColumnWins = label9;
                BettingWindowLayout bettingWindowLayout2 = bettingWindowLayout1;
                collection[index2] = bettingWindowLayout2;
                int num = index1 + 1;
                index1 = num > 5 ? 0 : num;
            }
            TableLayout = new ObservableCollection<BettingWindowLayout>(collection);

            _countLabelService.ClearPrevSpinCountNumbers();
            _countLabelService.AddPrevSpinCountNumbers(spinCounts.ToList());
            _countLabelService.ClearCountNumbers();
        }

        private Style GetNumberStyle(int i, int selectedBoard, BoardNumber num) => num.IsCount() ? GetCountNumberStyle(i, selectedBoard) : GetNormalNumberStyle(i, selectedBoard);

        private Style GetCountNumberStyle(int i, int selectedBoard) => i == selectedBoard ? firstRowCountLabel : defaultCountLabel;

        private Style GetNormalNumberStyle(int i, int selectedBoard) => i == selectedBoard ? firstRowLabel : defaultLabel;

        private void ProcessCountNumbers(BoardNumber[] numbers, int currentSpinType, Guid table, int currentSpin, bool reset)
        {
            foreach (var bnum in numbers)
            {
                if (bnum.IsCount())
                {
                    var cNum = new CounterNumber(bnum.Number, defaultCountTextBlock, table, currentSpin, reset, currentSpinType);
                    _countLabelService.AddCountNumber(cNum);
                }
            }
        }

        private Style GetCountNumberTextBlockStyle(int phaseSpinType, int lastSpinType)
        {
            if (phaseSpinType != lastSpinType)
                return otherCountTextBlock;
            else
                return defaultCountTextBlock;
        }

        private Style GetDefaultNumberTextBlock(int i, int selectedBoard)
        {
            if (i == selectedBoard)
                return defaultFirstRowTextBlock;
            else
                return defaultTextBlock;
        }

        private IEnumerable<BoardNumber> ReOrderedColumnNumbers(BoardColumn boardColumn) => boardColumn.Numbers.OrderBy(num => num.Number);
    }
}
