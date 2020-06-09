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
using System.IO;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.IO.IsolatedStorage;

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
        private void RollDieDM_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var;
            var = RollDieDM.Text;
        }
        private void DMRollDiceBtn_Click(object sender, RoutedEventArgs e)
        {
            String diecheck = RollDieDM.Text;
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
                MessageBox.Show($"{thisRoll.Roll() } {RollDieDM.Text }");   // we will make this talk out to a rolling chat box in a sec
            }
            if (Regex.IsMatch(diecheck, rex) == false)
            {
                MessageBox.Show($"Not a valid input to roll dice, should be in the form of 3d6 , 4D8, 1d20 or even d20");
            }
        }
        private void Nameinput_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var2;
            var2 = Nameinput.Text;
        }

        private void CreateNewCharacter_Click(object sender, RoutedEventArgs e)
        {

            Character thischaracter = new Character();                                                // need to add check to exclude names that are identical to any already in party
            thischaracter.NewCharacter(thischaracter);
            thischaracter.Name = Nameinput.Text; 
            thischaracter.RerollCharacter(thischaracter);
            Party thisparty = (Party)GroupList.SelectedItem;                   // choosing the party as per selction on list; need to do something about no party chosen maybe go to default                                                                       
            thisparty.Add(thischaracter);
            thischaracter.PartyName = thisparty.Name;                          // on adding a character to a party change the character PartyName to selected party's name 
        }


        private void ListParty_Click(object sender, RoutedEventArgs e)
        {
            foreach (Party group in MainWindow.Parties)
            { MessageBox.Show($"{group.Name}"); }
        }

        private void ShwCharSht_Click(object sender, RoutedEventArgs e)
        {
            // Character thischaracter = new Character(Nameinput.Text);             //ok just mucking around to see if I could pass character in this fashion to next window
            //  ShowCharWin ShowCharWin1 = new ShowCharWin(MainWindow.Party[0] );      // this is temp, will obv. need to change character shown , want to see if it passes
            foreach (Character selected in CurrentPartyList.SelectedItems)
            {
                ShowCharWin ShowCharWin1 = new ShowCharWin(selected);
                ShowCharWin1.Show();
            }
        }
        private void UpdatePartiesButton_Click(object sender, RoutedEventArgs e)
        {
            List<Party> currentParties = new List<Party>();         // I think both this and update party button could actually be rigged to happen when characters or parties were added or selected in the appropriate spots but this will do for now 
            foreach (Party group in MainWindow.Parties)           //was MainWindow.Parties
            { currentParties.Add(group); }
            GroupList.ItemsSource = currentParties;
            GroupList.DisplayMemberPath = "Name";
        }
        private void UpdatePartyButton_Click(object sender, RoutedEventArgs e)
        {
            List<Character> currentparty = new List<Character>();
            Party thisparty = (Party)GroupList.SelectedItem;         //was MainWindow.Party.Add(thischaracter);                                 
            foreach (Character charac in thisparty)                  //was MainWindow.Party
            { currentparty.Add(charac); }
            CurrentPartyList.ItemsSource = currentparty;
            CurrentPartyList.DisplayMemberPath = "Name";

        }
        private void GroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupList.SelectionChanged += GroupList_SelectionChanged;
            List<Character> currentparty = new List<Character>();
            Party thisparty = (Party)GroupList.SelectedItem;                        //was MainWindow.Party.Add(thischaracter);                                 
            foreach (Character charac in thisparty)                                 //was MainWindow.Party
            { currentparty.Add(charac); }
            CurrentPartyList.ItemsSource = currentparty;
            CurrentPartyList.DisplayMemberPath = "Name";

        }
        private void CurrentPartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPartyList.SelectionChanged += CurrentPartyList_SelectionChanged;
        }

        private void CreateNewParty_Click(object sender, RoutedEventArgs e)
        {
            Party thisparty = new Party();
            thisparty.Name = Nameinput.Text;
            MainWindow.Parties.Add(thisparty);            // was MainWindow.Parties
        }

        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            Save(MainWindow.Parties);
        }
        private void LoadAll_Click(object sender, RoutedEventArgs e)
        {
         List<Party> newpartylist = LoadPartyList();
         MainWindow.Parties = newpartylist;                               // at this point I could assign each parties name as per the groups name of Member 0! I think, try it 
         foreach (Party thisparty in MainWindow.Parties )
            { thisparty.Name = thisparty[0].PartyName; }
        }
         private List<Party>  LoadPartyList()
         {
          string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            MainWindow.Parties = new List<Party>();
           XmlSerializer formatter = new XmlSerializer(MainWindow.Parties.GetType());
         FileStream aFile = new FileStream(path, FileMode.Open);
           byte[] buffer = new byte[aFile.Length];
               aFile.Read(buffer, 0, (int) aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (List<Party> )formatter.Deserialize(stream);           
          }
        private void Save(List<Party> partysave)
        {
            string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            FileStream outfile = File.Create(path);
            XmlSerializer formatter = new XmlSerializer(partysave.GetType());
            formatter.Serialize(outfile, partysave);
        }

        
    }
    
}
