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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Character> Party;
        public static List<Party> Parties;
        public static List<PhysObj> GlobalItems;
        public static Character characterSelected;
        public MainWindow()
        {
            GlobalItems = new List<PhysObj>();            
            Party = new List<Character>();
            Parties = new List<Party>();
            characterSelected = new Character();
            characterSelected.Name = "Default";
            characterSelected.IsAlive = true;
            characterSelected.CharType = "Fighter";
            characterSelected.AC = 9;
            characterSelected.Str = 3;
            characterSelected.Inte = 3;
            characterSelected.Wis = 3;
            characterSelected.Con = 3;
            characterSelected.Dex = 3;
            characterSelected.Chr = 3;
            characterSelected.Exp = 3;
            characterSelected.HitOn20 = 10;
            characterSelected.MaxHp = 1;
            characterSelected.PartyName = "Default";
            

            InitializeComponent();
            
        }

        private void RunDM_Click(object sender, RoutedEventArgs e)
        {
            DMMainWin dMMainWin1 = new DMMainWin();
            dMMainWin1.Show();
        }

        private void RunPlayer_Click(object sender, RoutedEventArgs e)
        {
            PlayerMain playerMain1 = new PlayerMain();
            playerMain1.Show();
        }
    }
}
