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
        public MainWindow()
        {
            GlobalItems = new List<PhysObj>();
            PhysObj sword = new PhysObj();
            PhysObj shield = new PhysObj();
            GlobalItems.Add (sword);          // Global Items will be loaded like character and groups at start as will spells
            GlobalItems.Add(shield);          // Doing stuff here to check functionality
            sword.Name = "Big sword";
            shield.Name = "crappy shield.";
            Party = new List<Character>();
            Parties = new List<Party>();
            
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
