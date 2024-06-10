// Decompiled with JetBrains decompiler
// Type: Infinity.Roulette.TableGameSettingDefaultGame
// Assembly: Infinity.Roulette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3CFA51E1-B583-4CD4-A8C2-1C8E6C514381
// Assembly location: C:\Users\ArnoHanekom\Downloads\Release_20221202_v2\Release_20221202_v2\Release\Infinity.Roulette.dll

using Infinity.Roulette.Containers;
using Infinity.Roulette.ViewModels;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Unity;


#nullable enable
namespace Infinity.Roulette
{
  public partial class TableGameSettingDefaultGame : Window
    { 
    private TableGameSettingDefaultGameViewModel gsDefaultGameTypeVM { get; set; }

    public TableGameSettingDefaultGame()
    {
      if (Container.container != null)
        gsDefaultGameTypeVM = Container.container.Resolve<TableGameSettingDefaultGameViewModel>();
      DataContext = gsDefaultGameTypeVM;
      InitializeComponent();
    }

    private void btnSaveDefaultGame_Click(object sender, RoutedEventArgs e)
    {
      gsDefaultGameTypeVM.SaveDefaultGameSetting();
      ((TableGameSetting) Owner).UpdateDefaultGameType();
      Close();
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();

  }
}
