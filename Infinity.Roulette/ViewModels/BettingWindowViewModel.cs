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

        private Style GetNumberStyle(BoardNumber boardNumber, int rowIdx, CounterNumber[] prevCounts, int currSpinType, Style otherCountStyle)
        {
            if (!boardNumber.IsCount())
            {
                if (rowIdx == 0)
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
            int index1 = Game.BoardLayouts[0].Matrix.CurrentColumn;

            //Task.Run(async() =>
            //{
            //    var spins = await _engineService.GetSpinEntitiesAsync(Game.TableUniqueId, Game.GameId);
            //    if (spins.Count > 0)
            //    {
            //        Debug.WriteLine($"SPINS: ");
            //        for (int i = 0; i < spins.Count; i++)
            //        {
            //            var spin = spins[i];
            //            Debug.WriteLine($"Id: {spin.Id}\tWinning Number: {spin.WinningNumber}");
            //            if (i == spins.Count - 2)
            //            {
            //                Debug.WriteLine($"");
            //                Debug.WriteLine($"PREV Counts:");
            //                foreach (var num in spin.CountNumbers)
            //                {
            //                    Debug.WriteLine($"Number: {num.ToString()}");
            //                }
            //                Debug.WriteLine($"");
            //            }
            //            if (i == spins.Count - 1)
            //            {
            //                Debug.WriteLine($"");
            //                Debug.WriteLine($"Counts:");
            //                foreach (var num in spin.CountNumbers)
            //                {
            //                    Debug.WriteLine($"Number: {num.ToString()}");
            //                }
            //                Debug.WriteLine($"");
            //            }
            //        }
            //    }
            //});



            var activePhase = Game.BoardLayouts[0].GetActiveSpinPhase();
            bool reset = Game.BoardLayouts[0].IsReset;
            if (reset)
            {
                _countColorService.ResetOtherStyle();
            }

            var otherStyle = _countColorService.GetOtherStyle();
            Style countOtherStyle = otherStyle is null ? defaultCountTextBlock : (Style)otherStyle;

            //var selectedStyleKey = ResourceHelper.KeyFromResource(countOtherStyle);
            BoardNumber[] spinResults = Game.BoardLayouts[0].Columns.SelectMany(c => c.Numbers).ToArray();
            ProcessCountNumbers(spinResults, activePhase.StartedBy.HasValue ? activePhase.StartedBy.Value : -1, Game.TableUniqueId, Game.CurrentSpinNo, reset);
            Guid gameTable = Game.TableUniqueId;
            var prevSpinCounts = _countLabelService.GetPreviousSpinCountNumbers(gameTable);
            var spinCounts = _countLabelService.GetCountNumbers(gameTable);

            //var countStyle = _engineService.GetGameBoardCountStyle(Game.TableUniqueId, Game.RouletteGameId);
            //var gameSpinType = _engineService.GetGameBoardSpinType(Game.TableUniqueId, Game.RouletteGameId);
            //var isBoardReset = _engineService.GetGameBoardIsReset(Game.TableUniqueId, Game.RouletteGameId);

            //Debug.WriteLine($"");
            //Debug.WriteLine($"");
            //Debug.WriteLine($"\t\t\tGame Info");
            //Debug.WriteLine($"Count Style: '{countStyle}'\tSpin type: {gameSpinType}\tBoard Reset: {isBoardReset}");            
            ////var openPhases = _engineService.GetOpenPhases(gameTable, Game.RouletteGameId);
            //Debug.WriteLine($"");
            //Debug.WriteLine($"\t\t\tGame Open Phases ({openPhases.Count})");
            //foreach (var openPhase in openPhases)
            //{
            //    Debug.WriteLine($"Phase: {openPhase.Id}\tSpin Type: {(GameType)openPhase.SpinType}\tEnd Type: {(GameType)openPhase.EndType}\tStart: {openPhase.Start}\tDrawn: {openPhase.Drawn}");
            //}
            //
            //var phases = _engineService.GetPhases(gameTable, Game.RouletteGameId);
            //Debug.WriteLine($"");
            //Debug.WriteLine($"\t\t\tGame Phases ({phases.Count})");
            //foreach (var phase in phases)
            //{
            //    Debug.WriteLine($"Phase: {phase.Id}\tSpin Type: {(GameType)phase.SpinType}\tEnd Type: {(GameType)phase.EndType}\tStart: {phase.Start}\tDrawn: {phase.Drawn}");
            //}
            //
            //var lastEnded = _engineService.GetLastEndedPhase(gameTable, Game.RouletteGameId);
            //Debug.WriteLine($"");
            //Debug.WriteLine($"\t\t\tLast Ended Phase");
            //if (lastEnded is not null)
            //{
            //    Debug.WriteLine($"Phase: {lastEnded.Id}\tSpin Type: {(GameType)lastEnded.SpinType}\tEnd Type: {(GameType)lastEnded.EndType}\tStart: {lastEnded.Start}\tDrawn: {lastEnded.Drawn}");
            //    _engineService.DrawPhase(lastEnded);
            //    Debug.WriteLine($"Phase: {lastEnded.Id}\tSpin Type: {(GameType)lastEnded.SpinType}\tEnd Type: {(GameType)lastEnded.EndType}\tStart: {lastEnded.Start}\tDrawn: {lastEnded.Drawn}");
            //}
            //
            //var current = _engineService.GetCurrentOpenPhase(gameTable, Game.RouletteGameId);
            //Debug.WriteLine($"");
            //Debug.WriteLine($"\t\t\tLast Ended Phase");
            //if (current is not null)
            //{
            //    Debug.WriteLine($"Phase: {current.Id}\tSpin Type: {(GameType)current.SpinType}\tEnd Type: {(GameType)current.EndType}\tStart: {current.Start}\tDrawn: {current.Drawn}");
            //    _engineService.DrawPhase(current);
            //    Debug.WriteLine($"Phase: {current.Id}\tSpin Type: {(GameType)current.SpinType}\tEnd Type: {(GameType)current.EndType}\tStart: {current.Start}\tDrawn: {current.Drawn}");
            //}
            //var gameInfo = _engineService.GetGameInfo(Game.TableUniqueId, Game.RouletteGameId);
            //if (gameInfo is null)
            //{
            //    Debug.WriteLine($"Error: No game info found for Table: {Game.TableUniqueId}, Game: {Game.RouletteGameId}");
            //}
            //else
            //{
            //    var winnerInfo = gameInfo.SpinInfo.SpinNumberDetail;
            //    Debug.WriteLine($"Winning Number: {winnerInfo.WinningNumber}\tBoard Number: {winnerInfo.GameNumber.Number}\tNumber board: {winnerInfo.BoardCode}");
            //    Debug.WriteLine($"Number Codes: {winnerInfo.CodeCount}\tEstimated Codes: {winnerInfo.EstimatedCodeCount}\tActual Codes: {winnerInfo.ActualCodeCount}");
            //    var fullGameInfo = JsonConvert.SerializeObject(gameInfo, Formatting.Indented);
            //    Debug.WriteLine("JSON FULL GAME INFO:");
            //    Debug.WriteLine($"{fullGameInfo}");
            //}
            //
            //var gameSwapInfo = _engineService.GetGameSwapInfo(Game.TableUniqueId, Game.RouletteGameId);
            //if (gameInfo is null)
            //{
            //    Debug.WriteLine($"Error: No game swap info found for Table: {Game.TableUniqueId}, Game: {Game.RouletteGameId}");
            //}
            //else
            //{
            //    Debug.WriteLine($"");
            //    Debug.WriteLine($"");
            //    Debug.WriteLine($"\t\t\tSWAP Info");
            //    var fullSwapInfo = JsonConvert.SerializeObject(gameSwapInfo, Formatting.Indented);
            //    Debug.WriteLine("JSON FULL GAME SWAP INFO:");
            //    Debug.WriteLine($"{fullSwapInfo}");
            //}

            for (int index2 = 0; index2 < 6; ++index2)
            {
                int code = Game.BoardLayouts[0].Columns[index1].Code;
                int codeWin = Game.BoardLayouts[0].CodeWins[index2];                
                KeyValuePair<int, int>? nullable = new KeyValuePair<int, int>?(Game.BoardLayouts[0].CodeWins.OrderByDescending(cw => cw.Value).FirstOrDefault());
                bool flag1 = false;
                if (index2 == 0 && nullable.HasValue)
                    flag1 = nullable.Value.Key == index2;
                int columnWins = Game.BoardLayouts[0].Columns[index1].ColumnWins;
                bool flag2 = Game.BoardLayouts[0].Columns.HighestColumnWin() == columnWins;
                BoardNumber[] array = ReOrderedColumnNumbers(Game.BoardLayouts[0].Columns[index1]).ToArray();
                Style[] rowNumbersStyles = new Style[6];
                rowNumbersStyles[0] = GetNumberStyle(array[0], index2, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[1] = GetNumberStyle(array[1], index2, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[2] = GetNumberStyle(array[2], index2, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[3] = GetNumberStyle(array[3], index2, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[4] = GetNumberStyle(array[4], index2, prevSpinCounts, Game.SpinType, countOtherStyle);
                rowNumbersStyles[5] = GetNumberStyle(array[5], index2, prevSpinCounts, Game.SpinType, countOtherStyle);

                TextBlock textBlock1 = new TextBlock()
                {
                    Text = string.Format("{0}", code),
                    Style = GetDefaultNumberTextBlock(index2)
                };
                
                TextBlock textBlock2 = new TextBlock()
                {
                    Text = string.Format("{0}", array[0].Number),
                    Style = rowNumbersStyles[0]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[0].Number, rowNumbersStyles[0]);
                
                TextBlock textBlock3 = new TextBlock()
                {
                    Text = string.Format("{0}", array[1].Number),
                    Style = rowNumbersStyles[1]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[1].Number, rowNumbersStyles[1]);

                TextBlock textBlock4 = new TextBlock()
                {
                    Text = string.Format("{0}", array[2].Number),
                    Style = rowNumbersStyles[2]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[2].Number, rowNumbersStyles[2]);

                TextBlock textBlock5 = new TextBlock()
                {
                    Text = string.Format("{0}", array[3].Number),
                    Style = rowNumbersStyles[3]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[3].Number, rowNumbersStyles[3]);

                TextBlock textBlock6 = new TextBlock()
                {
                    Text = string.Format("{0}", array[4].Number),
                    Style = rowNumbersStyles[4]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[4].Number, rowNumbersStyles[4]);

                TextBlock textBlock7 = new TextBlock()
                {
                    Text = string.Format("{0}", array[5].Number),
                    Style = rowNumbersStyles[5]
                };
                _countLabelService.UpdateCountNumberStyle(gameTable, array[5].Number, rowNumbersStyles[5]);

                TextBlock textBlock8 = new TextBlock()
                {
                    Text = string.Format("{0}", codeWin)
                };
                TextBlock textBlock9 = new TextBlock()
                {
                    Text = string.Format("{0}", columnWins)
                };
                BettingWindowLayout bettingWindowLayout1 = new BettingWindowLayout();
                Label label1 = new Label();
                label1.Content = textBlock1;
                label1.Style = index2 == 0 ? firstRowLabel : defaultLabel;
                bettingWindowLayout1.Code = label1;
                Label label2 = new Label();
                label2.Content = textBlock2;
                label2.Style = GetNumberStyle(index2, array[0]);
                bettingWindowLayout1.Number1 = label2;
                Label label3 = new Label();
                label3.Content = textBlock3;
                label3.Style = GetNumberStyle(index2, array[1]);
                bettingWindowLayout1.Number2 = label3;
                Label label4 = new Label();
                label4.Content = textBlock4;
                label4.Style = GetNumberStyle(index2, array[2]);
                bettingWindowLayout1.Number3 = label4;
                Label label5 = new Label();
                label5.Content = textBlock5;
                label5.Style = GetNumberStyle(index2, array[3]);
                bettingWindowLayout1.Number4 = label5;
                Label label6 = new Label();
                label6.Content = textBlock6;
                label6.Style = GetNumberStyle(index2, array[4]);
                bettingWindowLayout1.Number5 = label6;
                Label label7 = new Label();
                label7.Content = textBlock7;
                label7.Style = GetNumberStyle(index2, array[5]);
                bettingWindowLayout1.Number6 = label7;
                Label label8 = new Label();
                label8.Content = textBlock8;
                label8.Style = flag1 ? rowWinsHighestAndFirstLabel : rowWinsLabel;
                bettingWindowLayout1.RowWins = label8;
                Label label9 = new Label();
                label9.Content = textBlock9;
                label9.Style = flag2 ? tableWinsHighestLabel : tableWinsLabel;
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

        private Style GetNumberStyle(int i, BoardNumber num) => num.IsCount() ? GetCountNumberStyle(i) : GetNormalNumberStyle(i);

        private Style GetCountNumberStyle(int i) => i == 0 ? firstRowCountLabel : defaultCountLabel;

        private Style GetNormalNumberStyle(int i) => i == 0 ? firstRowLabel : defaultLabel;

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

        private Style GetDefaultNumberTextBlock(int i)
        {
            if (i == 0)
                return defaultFirstRowTextBlock;
            else
                return defaultTextBlock;
        }

        private IEnumerable<BoardNumber> ReOrderedColumnNumbers(BoardColumn boardColumn) => boardColumn.Numbers.OrderBy(num => num.Number);
    }
}
