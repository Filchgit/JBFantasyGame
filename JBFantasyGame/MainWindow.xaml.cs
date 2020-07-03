using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
        public static List<Character> CharParty;
        public static List<CharParty> CharParties;
        public static List<Entity> Party;
        public static List<Party> Parties;
        public static List<PhysObj> GlobalItems;
        public static Entity characterSelected;
        public MainWindow()
        {
            GlobalItems = new List<PhysObj>();
            CharParty = new List<Character>();
            CharParties = new List <CharParty>();
            Party = new List<Entity>();
            Parties = new List<Party>();
            characterSelected = new Entity();
            characterSelected.Name = "Default";
            characterSelected.IsAlive = true;         
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
