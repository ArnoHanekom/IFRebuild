// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.LayoutModels.GameSpinHistory
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


#nullable enable
namespace Infinity.Roulette.LayoutModels
{
  public class GameSpinHistory : List<Label>
  {
    private int[] redNumbers = new int[18]
    {
      1,
      3,
      5,
      7,
      9,
      12,
      14,
      16,
      18,
      19,
      21,
      23,
      25,
      27,
      30,
      32,
      34,
      36
    };
    private int[] blackNumbers = new int[18]
    {
      2,
      4,
      6,
      8,
      10,
      11,
      13,
      15,
      17,
      20,
      22,
      24,
      26,
      28,
      29,
      31,
      33,
      35
    };

    public GameSpinHistory(List<int> spinHistory) => spinHistory.ForEach(sh => Add(PrepareHistoryLabel(sh)));

    private Label PrepareHistoryLabel(int historyNumber)
    {
      Border border = new Border();
      border.Background = redNumbers.Contains(historyNumber) ? Brushes.Red : (Brush) Brushes.Black;
      border.BorderBrush = redNumbers.Contains(historyNumber) ? Brushes.Red : (Brush) Brushes.Black;
      border.BorderThickness = new Thickness(1.0, 1.0, 1.0, 1.0);
      border.CornerRadius = new CornerRadius(5.0, 5.0, 5.0, 5.0);
      border.Width = 30.0;
      border.Height = 30.0;
      TextBlock textBlock1 = new TextBlock();
      textBlock1.Text = string.Format("{0}", historyNumber);
      textBlock1.HorizontalAlignment = HorizontalAlignment.Center;
      textBlock1.VerticalAlignment = VerticalAlignment.Center;
      textBlock1.FontWeight = FontWeights.DemiBold;
      TextBlock textBlock2 = textBlock1;
      border.Child = textBlock2;
      Label label = new Label();
      label.Content = border;
      label.HorizontalContentAlignment = HorizontalAlignment.Center;
      label.VerticalContentAlignment = VerticalAlignment.Center;
      label.Margin = new Thickness(0.0, 5.0, 0.0, 0.0);
      return label;
    }
  }
}
