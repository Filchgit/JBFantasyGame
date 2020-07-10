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
        Party Meleegroup = new Party();
        Party outofMeleeGroup = new Party(); 

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
        { if ((Party)EntGroupList.SelectedItem is null)                         // if ((CharParty)GroupList.SelectedItem is null)
            { MessageBox.Show("You must pick a party to add a new character. "); }
            else
            {
                Character thischaracter = new Character();             // might need to add check to exclude names that are identical to any already in party
                thischaracter.NewCharacter(thischaracter);
                thischaracter.Name = Nameinput.Text;           
                Party thisparty = (Party)EntGroupList.SelectedItem;
                int index = EntGroupList.SelectedIndex;
                CharParty charpartythis = MainWindow.CharParties[index];
                //CharParty charpartythis = (CharParty)GroupList.SelectedItem;      // this works but have to manually change it there
                thischaracter.PartyName = thisparty.Name;                          // on adding a character to a party change the character PartyName to selected party's name 
                charpartythis.Add(thischaracter);
                thisparty.Add(thischaracter);
                UpdatePartyListBox();
                UpdateTargetFocusCharListBox();
                // going to try and add simultaneously to an Entity party

                  UpdateEntPartyListBox();
            }
        }
        private void QuickCreateMonster_Click(object sender, RoutedEventArgs e)
        {
            if ((Party)EntGroupList.SelectedItem is null)
            { MessageBox.Show("You must pick a party to add a new Monster. "); }
            else
            {
                Monster thisMonster = new Monster();             // might need to add check to exclude names that are identical to any already in party
                thisMonster.NewMonster(thisMonster);  
                thisMonster.Name = Nameinput.Text;
                Party thisparty = (Party)EntGroupList.SelectedItem;
                int index = EntGroupList.SelectedIndex;
                MonsterParty monstPartyThis = MainWindow.MonsterParties[index];  
                
                thisMonster.PartyName = thisparty.Name;        // on adding a character to a party change the character PartyName to selected party's name 
                thisparty.Add(thisMonster);
                monstPartyThis.Add(thisMonster);
                UpdateMonstPartyListBox();
                UpdateEntPartyListBox();
                //  UpdateTargetFocusCharListBox();
            }
        }
        private void ShwCharSht_Click(object sender, RoutedEventArgs e)
        {
            foreach (Entity selected in EntCurrentPartyList.SelectedItems)
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
            if (EntCurrentPartyList.SelectedItem is null)
            { MessageBox.Show("You must pick a character to assign the item to. "); }
            else if (EntCurrentPartyList.SelectedItems.Count >1)
            { MessageBox.Show("You can only assign an item to one character, pick just one character and try again. "); }
            else
            { 
            Entity selected = (Entity)EntCurrentPartyList.SelectedItem;    
            PhysObj selectedobj = ((PhysObj)GlobalItems.SelectedItem);         
            selected.Inventory.Add(selectedobj);
            int itemind = GlobalItems.SelectedIndex;
            MainWindow.GlobalItems.RemoveAt(itemind);
            UpdateGlobalItems();
             }                                                                      
        }
        private void TransfertoNewParty_Click(object sender, RoutedEventArgs e)
        {
            if (EntCurrentPartyList.SelectedItem is null)                  // note that this only transfers one character at a time at the moment
            { MessageBox.Show("You must pick an Entity to transfer. "); }                  
            else
            {
                Entity selected = (Entity)EntCurrentPartyList.SelectedItem;
                Party toNewParty = (Party)TargetFocusGroupList.SelectedItem;
                //    Party oldParty = (Party)EntGroupList.SelectedItem;
                //  int oldInd = EntCurrentPartyList.SelectedIndex; 
                string oldPartyName = selected.PartyName;
                string newPartyName = toNewParty.Name;
                selected.PartyName = newPartyName;
                if (selected is Monster)
                {
                    foreach (MonsterParty monsterPartyThis in MainWindow.MonsterParties)
                    {
                        if (monsterPartyThis.Name == oldPartyName)
                            monsterPartyThis.Remove((Monster)selected);
                    }
                }
                if (selected is Character)
                {
                    foreach (CharParty charPartythis in MainWindow.CharParties)
                    {
                        if (charPartythis.Name == oldPartyName)
                            charPartythis.Remove((Character)selected);
                    }
                }
                    foreach (Party entParty in MainWindow.Parties )
                {  if (entParty.Name == oldPartyName)
                        entParty.Remove((Entity)selected);  }

                if (selected is Monster)                                                 
                { foreach (MonsterParty monsterPartyThis in MainWindow.MonsterParties)
                        if (monsterPartyThis.Name == newPartyName)
                            monsterPartyThis.Add((Monster)selected);   }
                if (selected is Character)
                { foreach (CharParty charPartyThis in MainWindow.CharParties)
                        if (charPartyThis.Name == newPartyName)
                            charPartyThis.Add((Character)selected);
                }
                toNewParty.Add(selected);                                
                //oldParty.RemoveAt(oldInd);                         
                UpdatePartyListBox();
                UpdateEntPartyListBox();
                UpdateMonstPartyListBox();
                UpdateTargetFocusCharListBox();
                
            }
        }
        private void Meleethis_Click(object sender, RoutedEventArgs e)              // this was a temp test 
        {
            Entity attacker = (Entity)EntCurrentPartyList.SelectedItem;
            Entity defender = (Entity)TargetFocusCharList.SelectedItem;
            attacker.MeleeAttack(defender);             
        }
        public void UpdatePartiesListBox()
        {
            List<CharParty> currentCharParties = new List<CharParty>();         // I think both this and update party button could actually be rigged to happen when characters or parties were added or selected in the appropriate spots but this will do for now 
            foreach (CharParty group in MainWindow.CharParties)             //was MainWindow.Parties
            { currentCharParties.Add(group); }
            GroupList.ItemsSource = currentCharParties;
            GroupList.DisplayMemberPath = "Name";
        }
        public void UpdatePartyListBox()
        {
            List<Entity> currentparty = new List<Entity>();
            CharParty thisparty = (CharParty)GroupList.SelectedItem;                                              
            foreach (Entity ent in thisparty)                       
            { currentparty.Add(ent); }
            CurrentPartyList.ItemsSource = currentparty;
            CurrentPartyList.DisplayMemberPath = "Name";
        }
        private void UpdateMonstPartiesListBox()
        {
            List<MonsterParty> currentMonstParties = new List<MonsterParty>();
            foreach (MonsterParty group in MainWindow.MonsterParties)
            { currentMonstParties.Add(group); }
            MonstGroupList.ItemsSource = currentMonstParties;
            MonstGroupList.DisplayMemberPath = "Name";
        }
        private void UpdateMonstPartyListBox()
        {
            List<Entity> currentparty = new List<Entity>();
            MonsterParty thisparty = (MonsterParty)MonstGroupList.SelectedItem;
            foreach (Entity ent in thisparty)
            { currentparty.Add(ent); }
            MonstCurrentPartyList.ItemsSource = currentparty;
            MonstCurrentPartyList.DisplayMemberPath = "Name";
        }
        private void UpdateEntPartyListBox()
        {
            List<Entity> currentparty = new List<Entity>();
            Party thisparty = (Party)EntGroupList.SelectedItem;
            foreach (Entity ent in thisparty)
            { currentparty.Add(ent); }
            EntCurrentPartyList.ItemsSource = currentparty;
            EntCurrentPartyList.DisplayMemberPath = "Name";
        }
        private void UpdateEntPartiesListBox()
        {
            List<Party> currentEntParties = new List<Party>();
            foreach (Party group in MainWindow.Parties)
            {if (group.Name != null )
                currentEntParties.Add(group); }
            EntGroupList.ItemsSource = currentEntParties;
            EntGroupList.DisplayMemberPath = "Name";

        }
        private void UpdateTargetFocusGroupListBox()
        {
            List<Party> currentParties = new List<Party>();        
            foreach (Party group in MainWindow.Parties)           
            { if (group.Name != null)
             currentParties.Add(group); }
            TargetFocusGroupList.ItemsSource = currentParties;
            TargetFocusGroupList.DisplayMemberPath = "Name";
        }
        private void UpdateTargetFocusCharListBox()
        {
                List<Entity> currentparty = new List<Entity>();
                 Party thisparty = (Party)TargetFocusGroupList.SelectedItem;              //was MainWindow.Party.Add(thischaracter);                                 
                 foreach (Entity entThis in thisparty)                                //was MainWindow.Party
                 { currentparty.Add(entThis); }
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
            MainWindow.entitySelected = (Entity)CurrentPartyList.SelectedItem;
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
            CharParty charPartyThis = new CharParty();
            Party entPartyThis = new Party();
            MonsterParty monsterPartyThis = new MonsterParty();
            charPartyThis.Name = Nameinput.Text;
            entPartyThis.Name = Nameinput.Text;
            monsterPartyThis.Name = Nameinput.Text; 
            MainWindow.CharParties.Add(charPartyThis);
            MainWindow.Parties.Add(entPartyThis);
            MainWindow.MonsterParties.Add(monsterPartyThis);         
            UpdatePartiesListBox();
            UpdateEntPartiesListBox();
            UpdateMonstPartiesListBox();
            UpdateTargetFocusGroupListBox();
        }     
        private void LoadAllFileDialog_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            string monstpath = "";            //this bit just opens file dialog
            var openFileDialog = new OpenFileDialog
            { Filter = "Text documents (.txt)|*.txt|Log files(.log)|*.log" };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult== true)
            {  
              monstpath = openFileDialog.FileName;
                path = openFileDialog.FileName;                                    
              char[] MyCharc = { '.', 't', 'x', 't' };
                string newString = path.TrimEnd(MyCharc);   
                path = newString  + " Char.txt";
            }                                                                    
            List<CharParty> newpartylist = LoadPartyList(path);                  
             MainWindow.CharParties = newpartylist;
             foreach (CharParty thisparty in MainWindow.CharParties )
                 {if (thisparty.Count !=0  )                                //actually need a placeholder maybe in case no member exists in mixed party
               thisparty.Name = thisparty[0].PartyName; }         
                        
            List<MonsterParty> newmonsterList = LoadMonstPartyList(monstpath);
            MainWindow.MonsterParties = newmonsterList;
           foreach (MonsterParty thisparty in MainWindow.MonsterParties)
            
                { if (thisparty.Count != 0)
                       thisparty.Name = thisparty[0].PartyName; }
            UpdateMonstPartiesListBox();
            UpdatePartiesListBox();
            foreach (CharParty nameCheckParty in MainWindow.CharParties)
            {
                bool charPartyNameExists = nameCheckParty.Any(p => p.Name != null);
                if (charPartyNameExists == false)
                {
                    int indexNoName = MainWindow.CharParties.IndexOf(nameCheckParty);
                    string nameToAssign = MainWindow.MonsterParties[indexNoName].Name;
                    nameCheckParty.Name = nameToAssign;
                }
            }
            foreach (MonsterParty nameCheckParty in MainWindow.MonsterParties)
            {
                bool charPartyNameExists = nameCheckParty.Any(p => p.Name != null);
                if (charPartyNameExists == false)
                {
                    int indexNoName = MainWindow.MonsterParties.IndexOf(nameCheckParty);
                    string nameToAssign = MainWindow.CharParties[indexNoName].Name;
                    nameCheckParty.Name = nameToAssign;
                }
            }
            foreach (CharParty thisCharParty in MainWindow.CharParties)
            {
                string partyNameToCheck = "";
                partyNameToCheck = thisCharParty.Name;
                Party thisNewParty = new Party();
                thisNewParty.Name = partyNameToCheck;
                MainWindow.Parties.Add(thisNewParty);
  
                foreach (Entity charEntity in thisCharParty  )
                { thisNewParty.Add(charEntity);  }
                foreach (MonsterParty thisMonsterParty in MainWindow.MonsterParties )
                { if (thisMonsterParty.Name == partyNameToCheck)
                    { foreach (Entity thisMonsterEnt in thisMonsterParty)
                        thisNewParty.Add(thisMonsterEnt); }
                }                            
            }
   
           
            UpdateEntPartiesListBox();
            UpdateEntPartyListBox();
            UpdateTargetFocusGroupListBox();
            UpdateTargetFocusCharListBox();
           
        }
        private List<CharParty> LoadPartyList (string path)
         {
         // string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            MainWindow.CharParties = new List<CharParty>();                                 
           XmlSerializer formatter = new XmlSerializer(MainWindow.CharParties.GetType());
         FileStream aFile = new FileStream(path, FileMode.Open);
           byte[] buffer = new byte[aFile.Length];
               aFile.Read(buffer, 0, (int) aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (List<CharParty> )formatter.Deserialize(stream);           
          }
        private List<MonsterParty> LoadMonstPartyList (string monstpath)
        {
            MainWindow.MonsterParties = new List<MonsterParty>();
            XmlSerializer formatter = new XmlSerializer(MainWindow.MonsterParties.GetType());
            FileStream aFile = new FileStream(monstpath, FileMode.Open);
            byte[] buffer = new byte[aFile.Length];
            aFile.Read(buffer, 0, (int)aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (List<MonsterParty>)formatter.Deserialize(stream);
        }
        private void SaveAll_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            { Filter = "Text documents (.txt)|*.txt|Log files(.log)|*log" };
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                string path = saveFileDialog.FileName;
                string monstpath  = saveFileDialog.FileName;
                char[] MyCharc = { '.', 't', 'x', 't' };
                string newString = path.TrimEnd(MyCharc);   
                path = newString  + " Char.txt";
                Save(MainWindow.CharParties, path);
                MonstSave(MainWindow.MonsterParties, monstpath);
            }
        }
        private void Save(List<CharParty> partysave, string path)                       //saving and loading in XML format at the moment only to allow very fast iteration, will implement an SQL load /save at a later time to show I can do and also for ease of organization if data gets huge
        {
            //string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            FileStream outfile = File.Create(path);
            XmlSerializer formatter = new XmlSerializer(partysave.GetType());
            formatter.Serialize(outfile, partysave);
        }
        private void MonstSave(List<MonsterParty> partysave, string monstpath)                       //saving and loading in XML format at the moment only to allow very fast iteration, will implement an SQL load /save at a later time to show I can do and also for ease of organization if data gets huge
        {
            //string path = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\NewFantTest.txt";
            FileStream outfile = File.Create(monstpath);
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
            Entity entitySel = MainWindow.entitySelected;
            if (entitySel is Character)
            {
                DMUpdateChar DMUpdateChar1 = new DMUpdateChar();
                DMUpdateChar1.Show();
            }
            UpdateEntPartiesListBox();
            UpdateEntPartyListBox();
            

            UpdateTargetFocusGroupListBox();
            UpdateTargetFocusCharListBox();

        }
        private void CurrentPartyList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdatePartyListBox();
        }       
        private void ItemUpdateGlobals_Click(object sender, RoutedEventArgs e)
        {
            UpdateGlobalItems();
        }
        private void GroupsIntoCombat_Click(object sender, RoutedEventArgs e)
        {
            PutTwoGroupsinCombat();
        }
         public void PutTwoGroupsinCombat()        
        {
            Party partycombat = (Party)EntGroupList.SelectedItem;
            Party Defparty = (Party)TargetFocusGroupList.SelectedItem;

            foreach (Entity thisEntity in Defparty)
            {
                foreach (Entity AttEntity in partycombat)
                {
                    Target Targetnew = new Target();
                    Targetnew.Name = AttEntity.Name;
                    Targetnew.Hp = AttEntity.Hp;
                    Targetnew.PartyName = AttEntity.PartyName;  
                    thisEntity.MeleeTargets.Add(Targetnew);
                }
            }
            foreach (Entity thisEntity in partycombat )
            {
                foreach (Entity DefEntity in Defparty)
                {
                    Target Targetnew = new Target();
                    Targetnew.Name = DefEntity.Name;
                    Targetnew.Hp = DefEntity.Hp;
                    Targetnew.PartyName = DefEntity.PartyName;  
                    thisEntity.MeleeTargets.Add(Targetnew);
                }
            }
                       
            foreach (Entity thisEntity in Defparty)
            {
               Meleegroup.Add(thisEntity);   
            }
            foreach (Entity thisEntity in partycombat )
            {
                Meleegroup.Add(thisEntity);
            }          
         }
        private void GroupsOutOfCombat_Click(object sender, RoutedEventArgs e)
        {
            PutTwoGroupsOutOfCombat();
        }
        private void PutTwoGroupsOutOfCombat()
        {
            Party partycombat = (Party)EntGroupList.SelectedItem;
            Party Defparty = (Party)TargetFocusGroupList.SelectedItem;
            foreach (Entity thisEntity in Defparty)
            { foreach (Entity AttEntity in partycombat)
             {thisEntity.MeleeTargets.Clear();}
            }
            foreach (Entity thisEntity in partycombat)
            {foreach (Entity DefEntity in Defparty)
               { thisEntity.MeleeTargets.Clear();}
            }
        }
        private void NextCombatRound_Click(object sender, RoutedEventArgs e)
        {
            foreach (Entity thisEntity in Meleegroup)                   // note there is no function to clear melee group as yet, though that is easy 
            {  if (thisEntity is Character)                             // although I can put people out of range
                { Character characterthis = new Character();
                    characterthis =(Character)thisEntity;
                    characterthis.InitRecalc(characterthis);
                    thisEntity.InitMod = characterthis.InitMod;   
                }

                RollingDie d60 = new RollingDie(60, 1);
                thisEntity.InitRoll = (d60.Roll()+ thisEntity.InitMod ) ;                //can put individual adjustments in at this point later 
            }
            Meleegroup.Sort((x, y) => x.InitRoll.CompareTo(y.InitRoll));   // Awesome this function sorts my list based on the property InitRoll (lowest to highest)
            foreach (Entity thisEntity in Meleegroup)                      // lower is better on init roll
            {
                if (thisEntity.MyTargetParty != null)
                {
                    string targetParty = thisEntity.MyTargetParty;
                    string targetEntity = thisEntity.MyTargetEnt;
                    foreach (Entity entitytobeattacked in Meleegroup)
                    {
                        if (entitytobeattacked.PartyName == targetParty && entitytobeattacked.Name == targetEntity)
                        {
                            if (entitytobeattacked is Character)
                            {
                                Character attackasCharacter = new Character();
                                attackasCharacter = (Character)entitytobeattacked;
                                thisEntity.MeleeAttack(attackasCharacter);
                            }
                            else

                            { thisEntity.MeleeAttack(entitytobeattacked); }
                        }
                    }
                }
            }
            
        }       
        private void MonstGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MonstGroupList.SelectionChanged += MonstGroupList_SelectionChanged;
            UpdateMonstPartyListBox();     

        }
        private void MonstCurrentPartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MonstCurrentPartyList.SelectionChanged += MonstCurrentPartyList_SelectionChanged;
            MainWindow.entitySelected = (Monster)CurrentPartyList.SelectedItem;
            UpdateMonstPartyListBox();
        }
        private void MonstCurrentPartyList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdateMonstPartyListBox();
        }
        private void EntCurrentPartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EntCurrentPartyList.SelectionChanged += EntCurrentPartyList_SelectionChanged;
            MainWindow.entitySelected = (Entity)EntCurrentPartyList.SelectedItem;
            UpdateEntPartyListBox();
        }
        private void EntCurrentPartyList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdateEntPartyListBox();
        }
        private void EntGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EntGroupList.SelectionChanged += EntGroupList_SelectionChanged;          
            UpdateEntPartyListBox();
        }

        private void EntGroupList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdateEntPartyListBox();
        }

        private void DeleteEntity_Click(object sender, RoutedEventArgs e)
        {
            Entity selected = MainWindow.entitySelected; ;
            string removeFromGroup = selected.PartyName;           
            if (selected is Character)
            { foreach (CharParty thisCharParty in MainWindow.CharParties)
                if(thisCharParty.Name == removeFromGroup)
                    { thisCharParty.Remove((Character)selected );  }}
            if (selected is Monster)
            {foreach (MonsterParty thisMonstParty in MainWindow.MonsterParties)
                    if (thisMonstParty.Name == removeFromGroup)
                    { thisMonstParty.Remove((Monster)selected); }
            }
            foreach (Party thisEntParty in MainWindow.Parties)
                if (thisEntParty.Name == removeFromGroup)
                { thisEntParty.Remove(selected); }
            UpdateEntPartiesListBox();
            UpdateEntPartyListBox();
            UpdateTargetFocusGroupListBox();
            UpdateTargetFocusCharListBox();
        }
    }
    
}
