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
using System.ComponentModel.Design.Serialization;
using Microsoft.Win32;

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
            UpdateGlobalItems();           
        }
        private void RollDieDM_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var;
            var = RollDieDM.Text;
        }
        private void DMRollDiceBtn_Click(object sender, RoutedEventArgs e)
        {
            String diecheck = RollDieDM.Text;
            (int i1, int i2, int i3)= RollingDie.Diecheck(diecheck);
            if (i1 != 0)
            {
                RollingDie thisRoll = new RollingDie(i1, i2, i3);
                string rollexp = thisRoll.ToString();  
                MessageBox.Show($"{thisRoll.Roll() } {rollexp}  {RollDieDM.Text }");   // we will make this talk out to a rolling chat box in a sec
            }
        }
        private void Nameinput_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var2;                      //I don't think I need this; put in when I was going to error check
            var2 = Nameinput.Text;
        }
        private void CreateNewCharacter_Click(object sender, RoutedEventArgs e)
        { if ((Party)GroupList.SelectedItem is null)
            { MessageBox.Show("You must pick a party to add a new character. "); }
            else
            {
                Character thischaracter = new Character();             // might need to add check to exclude names that are identical to any already in party
                thischaracter.NewCharacter(thischaracter);
                thischaracter.Name = Nameinput.Text;
               
                Party thisparty = (Party)GroupList.SelectedItem;                                                                                          
                thisparty.Add(thischaracter);
                thischaracter.PartyName = thisparty.Name;                          // on adding a character to a party change the character PartyName to selected party's name 
                UpdatePartyListBox();
                UpdateTargetFocusCharListBox();
            }
        }
        private void ShwCharSht_Click(object sender, RoutedEventArgs e)
        {
            foreach (Entity selected in CurrentPartyList.SelectedItems)
            {   if (selected is Character)
                {
                    ShowCharWin ShowCharWin1 = new ShowCharWin((Character)selected);
                    ShowCharWin1.Show();
                }
                else;//  stub for putting in a monster char sheet 
            }
        }
        public void UpdateGlobalItems()
        {
            List<PhysObj> currentPhysObj = new List<PhysObj>();
            foreach (PhysObj physthing in MainWindow.GlobalItems)
            { currentPhysObj.Add(physthing); }
            GlobalItems.ItemsSource = currentPhysObj;
            GlobalItems.DisplayMemberPath = "Name";
        }
        private void AssigntoCharacter_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPartyList.SelectedItem is null)
            { MessageBox.Show("You must pick a character to assign the item to. "); }
            else if (CurrentPartyList.SelectedItems.Count >1)
            { MessageBox.Show("You can only assign an item to one character, pick just one character and try again. "); }
            else
            { 
            Entity selected = (Entity)CurrentPartyList.SelectedItem;    
            PhysObj selectedobj = ((PhysObj)GlobalItems.SelectedItem);         
            selected.Inventory.Add(selectedobj);
            int itemind = GlobalItems.SelectedIndex;
            MainWindow.GlobalItems.RemoveAt(itemind);
            UpdateGlobalItems();
             }                                                                      
        }
        private void TransfertoNewParty_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPartyList.SelectedItem is null)                  // note that this only transfers one character at a time at the moment
            { MessageBox.Show("You must pick an Entity to transfer. "); }       // only works for character not entitys at present           
            else
            {
                Entity selected = (Entity)CurrentPartyList.SelectedItem;
                Party toNewParty = (Party)TargetFocusGroupList.SelectedItem;
                Party oldParty = (Party)GroupList.SelectedItem;
                int oldInd = CurrentPartyList.SelectedIndex;
                toNewParty.Add(selected);      // whyis this saying it must be converted??
                oldParty.RemoveAt(oldInd);
                UpdatePartyListBox();
                UpdateTargetFocusCharListBox();
            }
        }
        private void Meleethis_Click(object sender, RoutedEventArgs e)
        {
            Character attacker = (Character)CurrentPartyList.SelectedItem;
            Character defender = (Character)TargetFocusCharList.SelectedItem;
            attacker.MeleeAttack(defender);             
        }
        public void UpdatePartiesListBox()
        {
            List<Party> currentParties = new List<Party>();         // I think both this and update party button could actually be rigged to happen when characters or parties were added or selected in the appropriate spots but this will do for now 
            foreach (Party group in MainWindow.Parties)             //was MainWindow.Parties
            { currentParties.Add(group); }
            GroupList.ItemsSource = currentParties;
            GroupList.DisplayMemberPath = "Name";
        }
        public void UpdatePartyListBox()
        {
            List<Entity> currentparty = new List<Entity>();
            Party thisparty = (Party)GroupList.SelectedItem;              //was MainWindow.Party.Add(thischaracter);                                 
            foreach (Entity charac in thisparty)                       //was MainWindow.Party
            { currentparty.Add(charac); }
            CurrentPartyList.ItemsSource = currentparty;
            CurrentPartyList.DisplayMemberPath = "Name";
        }
        private void UpdateTargetFocusGroupListBox()
        {
            List<Party> currentParties = new List<Party>();         // need to add update UpdateTargetFocusGroupListBox() in all appropriat places 
            foreach (Party group in MainWindow.Parties)           
            { currentParties.Add(group); }
            TargetFocusGroupList.ItemsSource = currentParties;
            TargetFocusGroupList.DisplayMemberPath = "Name";
        }
        private void UpdateTargetFocusCharListBox()
        {
                List<Entity> currentparty = new List<Entity>();
                 Party thisparty = (Party)TargetFocusGroupList.SelectedItem;              //was MainWindow.Party.Add(thischaracter);                                 
                 foreach (Entity charac in thisparty)                                //was MainWindow.Party
                 { currentparty.Add(charac); }
                 TargetFocusCharList.ItemsSource = currentparty;
                TargetFocusCharList.DisplayMemberPath = "Name";
        }
        private void GroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupList.SelectionChanged += GroupList_SelectionChanged;
            UpdatePartyListBox();
        }
        private void CurrentPartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentPartyList.SelectionChanged += CurrentPartyList_SelectionChanged;
            MainWindow.characterSelected = (Entity)CurrentPartyList.SelectedItem;
            UpdatePartyListBox();

        }
        private void GlobalItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GlobalItems.SelectionChanged += GlobalItems_SelectionChanged;
        }
        private void TargetFocusGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TargetFocusGroupList.SelectionChanged += TargetFocusGroupList_SelectionChanged;
            UpdateTargetFocusCharListBox();
        }
        private void TargetFocusCharList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TargetFocusCharList.SelectionChanged += TargetFocusCharList_SelectionChanged;
        }
        private void CreateNewParty_Click(object sender, RoutedEventArgs e)
        {
            Party thisparty = new Party();
            thisparty.Name = Nameinput.Text;
            MainWindow.Parties.Add(thisparty);            // was MainWindow.Parties
            UpdatePartiesListBox();
            UpdateTargetFocusGroupListBox();
        }     
        private void LoadAllFileDialog_Click(object sender, RoutedEventArgs e)
        {
            string path = "";                                                      //this bit just opens file dialog
            var openFileDialog = new OpenFileDialog
            { Filter = "Text documents (.txt)|*.txt|Log files(.log)|*.log" };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult== true)
            {
               path = openFileDialog.FileName;                                    // and sets path to file that we open in dialog
            }
            List<Party> newpartylist = LoadPartyList(path);
             MainWindow.Parties = newpartylist;
             foreach (Party thisparty in MainWindow.Parties )
                 { thisparty.Name = thisparty[0].PartyName; }         
               UpdatePartiesListBox();
               UpdateTargetFocusGroupListBox();
              UpdateTargetFocusCharListBox();
        }
        private List<Party> LoadPartyList (string path)
         {
         // string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            MainWindow.Parties = new List<Party>();
           XmlSerializer formatter = new XmlSerializer(MainWindow.Parties.GetType());
         FileStream aFile = new FileStream(path, FileMode.Open);
           byte[] buffer = new byte[aFile.Length];
               aFile.Read(buffer, 0, (int) aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (List<Party> )formatter.Deserialize(stream);           
          }
        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            { Filter = "Text documents (.txt)|*.txt|Log files(.log)|*log" };
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                string path = saveFileDialog.FileName;
                Save(MainWindow.Parties, path); }
        }
        private void Save(List<Party> partysave, string path)                       //saving and loading in XML format at the moment only to allow very fast iteration, will implement an SQL load /save at a later time to show I can do and also for ease of organization if data gets huge
        {
            //string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            FileStream outfile = File.Create(path);
            XmlSerializer formatter = new XmlSerializer(partysave.GetType());
            formatter.Serialize(outfile, partysave);
        }
        private void QuickCreateObj_Click(object sender, RoutedEventArgs e)
        {
            GlobalItemAdd GlobalItemAdd1 = new GlobalItemAdd();
            GlobalItemAdd1.Show();
        }
        private void DmAdjustChar_Click(object sender, RoutedEventArgs e)
        {           
                    DMUpdateChar DMUpdateChar1 = new DMUpdateChar();
                    DMUpdateChar1.Show();                             
        }
        private void CurrentPartyList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdatePartyListBox();
        }
        private void ItemUpdateGlobals_Click(object sender, RoutedEventArgs e)
        {
            UpdateGlobalItems();
        }
        private void GroupCombat_Click(object sender, RoutedEventArgs e)
        {
            GroupCombatSequence();
        }
         public void GroupCombatSequence()
        {
            Party partycombat = (Party)GroupList.SelectedItem;
            Party Defparty = (Party)TargetFocusGroupList.SelectedItem;
           
            foreach (Entity thisEntity in Defparty)
            {
                partycombat.Add((Character)thisEntity);   
            }
            foreach (Entity thisEntity in partycombat)
            {
                RollingDie d60 = new RollingDie(60, 1);
                thisEntity.InitRoll = d60.Roll();                //can put individual adjustments in at this point later 
            }      
            partycombat.Sort((x, y) => x.InitRoll.CompareTo(y.InitRoll));   // Awesome this function sorts my list based on the property InitRoll (lowest to highest)
            var initiativeDeclare = "";
 
            foreach (Entity thisEntity in partycombat)
            {
                string thisEntityName = thisEntity.Name.ToString();
                string initRoll = thisEntity.InitRoll.ToString();
                initiativeDeclare += (thisEntityName + " " + initRoll + " ");
            }
            MessageBox.Show(initiativeDeclare);
        }
    }
    
}
