﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Character> playcharacter;
        public MainWindow()
        {
            playcharacter = new List<Character>();
            InitializeComponent();
            
        }

        private void RunDM_Click(object sender, RoutedEventArgs e)
        {
            DMMainWin dMMainWin1 = new DMMainWin();
            dMMainWin1.Show();
        }
    }
}
