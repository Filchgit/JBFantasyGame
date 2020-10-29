using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public static List<Monster> MonsterParty;
        public static List<MonsterParty> MonsterParties;
        public static List<Fant_Entity> Party;
        public static List<Party> Parties;
        public static List<PhysObj > GlobalItems;
        public static Fant_Entity entitySelected;
                public MainWindow()
        {
            GlobalItems = new List<PhysObj >();
            CharParty = new List<Character>();
            CharParties = new List <CharParty>();
            MonsterParty = new List<Monster>();
            MonsterParties = new List<MonsterParty>();
            Party = new List<Fant_Entity>();
            Parties = new List<Party>();
            entitySelected = new Fant_Entity();
            entitySelected.Name = "Default";
            entitySelected.IsAlive = true;
            entitySelected.HitOn20 = 10;
            entitySelected.MaxHp = 1;
            entitySelected.PartyName = "Default";

           // sqlEntitySelected = new Fant_Entity();
           // sqlEntitySelected.Name = "Default";
           // sqlEntitySelected.PartyName= "Default";
           // sqlEntitySelected.Lvl = 1;


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
