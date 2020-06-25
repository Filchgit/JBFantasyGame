﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ShowCharWin : Window
    {
        private Character showcharacter;
        private DispatcherTimer dispatcherTimer = null;

        public ShowCharWin(Character thischaracter)
        {
            InitializeComponent();
            showcharacter = thischaracter;
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1.0);
            dispatcherTimer.Tick += OnTimerTick;
            dispatcherTimer.Start();

            //
        }
        private void OnTimerTick(object sender, EventArgs e)
        { 
            ShowCharname.Text = showcharacter.Name.ToString();
            ShowCharHP.Text = showcharacter.Hp.ToString();
            ShowCharStr.Text = showcharacter.Str.ToString();  
            ShowCharInt.Text = showcharacter.Inte.ToString();
            ShowCharWis.Text = showcharacter.Wis.ToString();
            ShowCharDex.Text = showcharacter.Dex.ToString();
            ShowCharCon.Text = showcharacter.Con.ToString();
            ShowCharChr.Text = showcharacter.Chr.ToString();
            ShowCharLvl.Text = showcharacter.Lvl.ToString();
            ShowCharExp.Text = showcharacter.Exp.ToString();
            ShowGroup.Text = showcharacter.PartyName.ToString();

             CharInv.ItemsSource = showcharacter.Inventory ;
             CharInv.DisplayMemberPath = "Name";

        }
    }
  
    //}
   // DataContext = CharacterDetails;

     //         public Character CharacterDetails
    //{
      //  get { return (Character)GetValue(CharacterDetailsProperty); }
        //set { SetValue(CharacterDetailsProperty, value); }
    //}
    //public static readonly DependencyProperty CharacterDetailsProperty = DependencyProperty.Register("CharacterDetails",
      //                  typeof(Character),
        //                typeof(ShowCharWin),
          //        new PropertyMetadata(null));


}

