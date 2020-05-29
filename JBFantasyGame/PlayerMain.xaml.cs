using System;
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
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for PlayerMain.xaml
    /// </summary>
    public partial class PlayerMain : Window
    {
        public PlayerMain()
        {
            InitializeComponent();
        }

        private void RollDiePlay_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void PlayRollDiceBtn_Click(object sender, RoutedEventArgs e)
        {
            String diecheck = RollDiePlay.Text;
            string rex = "^([0-9]*)[D-d]([0-9]+)";
            if (Regex.IsMatch(diecheck, rex) == true)
            {
                string[] splitdie = diecheck.Split(new Char[] { 'D', 'd' });
                int i1;
                int i2;
                if (splitdie[0] != "")
                {
                    i2 = Int32.Parse(splitdie[0]);
                    i1 = Int32.Parse(splitdie[1]);
                }
                else
                {
                    i1 = Int32.Parse(splitdie[1]);
                    i2 = 1;
                }

                RollingDie thisRoll = new RollingDie(i1, i2);
                MessageBox.Show($"{thisRoll.Roll() } {RollDiePlay.Text }");   // we will make this talk out to a rolling chat box in a sec
            }
            if (Regex.IsMatch(diecheck, rex) == false)
            {
                MessageBox.Show($"Not a valid input to roll dice, should be in the form of 3d6 , 4D8, 1d20 or even d20");
            }

        }

        private void RollDiePlay_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var;
            var = RollDiePlay.Text;
        }
    }
}
