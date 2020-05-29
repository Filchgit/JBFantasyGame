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

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for DMMainWin.xaml
    /// </summary>
    public partial class DMMainWin : Window
    {
        public DMMainWin()
        {
            InitializeComponent();
        }
        
     

        private void RollDieDM_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string var;
            var = RollDieDM.Text;
            MessageBox.Show($"{var}");
        }

        private void RollDieDM_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var;
            var = RollDieDM.Text;          
        }

        private void DMRollDiceBtn_Click(object sender, RoutedEventArgs e)
        {
            RollingDie thisRoll = new RollingDie(20,2);
            MessageBox.Show($"{thisRoll.Roll() }"); 
        }
    }
}
