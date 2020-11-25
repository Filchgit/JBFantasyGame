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
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.Security.Policy;
using System.Net;
using System.Windows.Threading;
using System.Threading;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for DMMainWin.xaml
    /// </summary>
    public partial class DMMainWin : Window
    {     // obviously will have option to change connection for other people
        SqlConnection con = new SqlConnection(@"Data Source = JBLAPTOP\SQLEXPRESS; Initial Catalog = FantasyGame; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");
        string comScriptPath = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\combatScript.txt";
        Party Meleegroup = new Party();
        public Character checkCharacter = new Character();
        //Stuff for TCP Server
        JBSocketServer myServer;
        int portNumb = 50000;
        JBSocketServer myServerCommands;
        int portNumbCom = 50000;
        string finalStringRecCom = "";
        string finalString = "";
        string xML = "";
        private DispatcherTimer dispatcherTimerCom = null;
        public DMMainWin()
        {
            InitializeComponent();
            UpdateGlobalItems();
            //stuff for TCP Server
            myServer = new JBSocketServer();
            myServerCommands = new JBSocketServer();

            myServer.RaiseClientConnectedEvent += HandleClientConnected;
            myServer.RaiseTextReceivedEvent += HandleTextReceived;

            myServerCommands.RaiseClientConnectedEvent += HandleClientComConnected;
            myServerCommands.RaiseTextReceivedEvent += HandleTextComReceived;

            string hostName = Dns.GetHostName();
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            txtMyIPAddress.Text = myIP;
            dispatcherTimerCom = new DispatcherTimer();
            dispatcherTimerCom.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimerCom.Tick += OnTimerTickCom;

        }

        private void RollDieDM_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var;
            var = RollDieDM.Text;
        }
        private void DMRollDiceBtn_Click(object sender, RoutedEventArgs e)
        {
            String diecheck = RollDieDM.Text;
            (int i1, int i2, int i3) = RollingDie.Diecheck(diecheck);
            if (i1 != 0)
            {
                RollingDie thisRoll = new RollingDie(i1, i2, i3);
                string rollexpl = thisRoll.ToString();
                MessageBox.Show($"{thisRoll.Roll() } {rollexpl}  {RollDieDM.Text }");   // we will make this talk out to a rolling chat box in a sec
            }
        }
        private void Nameinput_TextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void CreateNewCharacter_Click(object sender, RoutedEventArgs e)
        {
            if ((Party)EntGroupList.SelectedItem is null)
            { MessageBox.Show("You must pick a party to add a new character. "); }
            else
            {
                bool invalidName = false;
                string checkName = "";
                checkName = Nameinput.Text;
                invalidName = checkName.Contains("|");
                if (invalidName == false)
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


                    UpdateEntPartyListBox();
                }
                else { MessageBox.Show("Invalid name - contains |"); }
            }
        }
        private void QuickCreateMonster_Click(object sender, RoutedEventArgs e)
        {
            if ((Party)EntGroupList.SelectedItem is null)
            { MessageBox.Show("You must pick a party to add a new Monster. "); }
            else
            {
                bool invalidName = false;
                string checkName = "";
                checkName = Nameinput.Text;
                invalidName = checkName.Contains("|");
                if (invalidName == false)
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
                }                                 //  UpdateTargetFocusCharListBox();              maybe I should just do an updateAllListBoxes 
                else { MessageBox.Show("Invalid name - contains |"); }
            }
        }
        private void ShwCharSht_Click(object sender, RoutedEventArgs e)
        {
            foreach (Fant_Entity selected in EntCurrentPartyList.SelectedItems)
            {   //without pattern matching the first one is if (selected is Character)
                // ShowCharWin ShowCharWin1 = new ShowCharWin((Character)selected)
                if (selected is Character character)
                {
                    ShowCharWin ShowCharWin1 = new ShowCharWin(character);
                    ShowCharWin1.Show();
                }
                if (selected is Monster monster)
                {
                    ShowMonsterWin ShowMonsterWin1 = new ShowMonsterWin(monster);
                    ShowMonsterWin1.Show();
                }
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
            else if (EntCurrentPartyList.SelectedItems.Count > 1)
            { MessageBox.Show("You can only assign an item to one character, pick just one character and try again. "); }
            else
            {
                Fant_Entity selected = (Fant_Entity)EntCurrentPartyList.SelectedItem;
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
            { MessageBox.Show("You must pick an Fant_Entity to transfer. "); }
            else
            {
                Fant_Entity selected = (Fant_Entity)EntCurrentPartyList.SelectedItem;
                Party toNewParty = (Party)TargetFocusGroupList.SelectedItem;
                string oldPartyName = selected.PartyName;
                string newPartyName = toNewParty.Name;
                selected.PartyName = newPartyName;
                if (selected is Monster monster)
                {
                    foreach (MonsterParty monsterPartyThis in MainWindow.MonsterParties)
                    {
                        if (monsterPartyThis.Name == oldPartyName)
                            monsterPartyThis.Remove(monster);
                    }
                }
                if (selected is Character character)
                {
                    foreach (CharParty charPartythis in MainWindow.CharParties)
                    {
                        if (charPartythis.Name == oldPartyName)
                            charPartythis.Remove(character);
                    }
                }
                foreach (Party entParty in MainWindow.Parties)
                {
                    if (entParty.Name == oldPartyName)
                        entParty.Remove((Fant_Entity)selected);
                }

                if (selected is Monster)
                {
                    foreach (MonsterParty monsterPartyThis in MainWindow.MonsterParties)
                        if (monsterPartyThis.Name == newPartyName)
                            monsterPartyThis.Add((Monster)selected);
                }
                if (selected is Character)
                {
                    foreach (CharParty charPartyThis in MainWindow.CharParties)
                        if (charPartyThis.Name == newPartyName)
                            charPartyThis.Add((Character)selected);
                }
                toNewParty.Add(selected);
                UpdateAllListBoxes();
            }
        }
        private void Meleethis_Click(object sender, RoutedEventArgs e)              // this was a temp test 
        {
            Fant_Entity attacker = (Fant_Entity)EntCurrentPartyList.SelectedItem;
            Fant_Entity defender = (Fant_Entity)TargetFocusCharList.SelectedItem;
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
            List<Fant_Entity> currentparty = new List<Fant_Entity>();
            CharParty thisparty = (CharParty)GroupList.SelectedItem;
            if (thisparty != null)
            {
                foreach (Fant_Entity ent in thisparty)
                { currentparty.Add(ent); }
                CurrentPartyList.ItemsSource = currentparty;
                CurrentPartyList.DisplayMemberPath = "Name";
            }
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
            List<Fant_Entity> currentparty = new List<Fant_Entity>();
            MonsterParty thisparty = (MonsterParty)MonstGroupList.SelectedItem;
            if (thisparty != null)
            {
                foreach (Fant_Entity ent in thisparty)
                { currentparty.Add(ent); }
                MonstCurrentPartyList.ItemsSource = currentparty;
                MonstCurrentPartyList.DisplayMemberPath = "Name";
            }
        }
        private void UpdateEntPartyListBox()
        {
            List<Fant_Entity> currentparty = new List<Fant_Entity>();
            Party thisparty = (Party)EntGroupList.SelectedItem;
            foreach (Fant_Entity ent in thisparty)
            { currentparty.Add(ent); }
            EntCurrentPartyList.ItemsSource = currentparty;
            EntCurrentPartyList.DisplayMemberPath = "Name";
        }
        private void UpdateEntPartiesListBox()
        {
            List<Party> currentEntParties = new List<Party>();
            foreach (Party group in MainWindow.Parties)
            {
                if (group.Name != null)
                    currentEntParties.Add(group);
            }
            EntGroupList.ItemsSource = currentEntParties;
            EntGroupList.DisplayMemberPath = "Name";

        }
        private void UpdateTargetFocusGroupListBox()
        {
            List<Party> currentParties = new List<Party>();
            foreach (Party group in MainWindow.Parties)
            {
                if (group.Name != null)
                    currentParties.Add(group);
            }
            TargetFocusGroupList.ItemsSource = currentParties;
            TargetFocusGroupList.DisplayMemberPath = "Name";
        }
        private void UpdateTargetFocusCharListBox()
        {
            List<Fant_Entity> currentparty = new List<Fant_Entity>();
            Party thisparty = (Party)TargetFocusGroupList.SelectedItem;
            foreach (Fant_Entity entThis in thisparty)
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
            MainWindow.entitySelected = (Fant_Entity)CurrentPartyList.SelectedItem;
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
            bool partyExists = DoesPartyExistinSQL(Nameinput.Text);
            if (partyExists == false)
            {
                bool invalidName = false;
                string checkName = "";
                checkName = Nameinput.Text;
                invalidName = checkName.Contains("|");
                if (invalidName == false)
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
                    UpdateAllListBoxes();
                }
                else { MessageBox.Show("Invalid name - contains |"); }
            }
        }
        #region XML Save and Load
        private void LoadAllFileDialog_Click(object sender, RoutedEventArgs e)
        {
            string path = "";
            string monstpath = "";            //this bit just opens file dialog
            var openFileDialog = new OpenFileDialog
            { Filter = "Text documents (.txt)|*.txt|Log files(.log)|*.log" };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult == true)
            {
                monstpath = openFileDialog.FileName;
                path = openFileDialog.FileName;
                char[] MyCharc = { '.', 't', 'x', 't' };
                string newString = path.TrimEnd(MyCharc);
                path = newString + " Char.txt";
            }
            List<CharParty> newpartylist = LoadPartyList(path);
            MainWindow.CharParties = newpartylist;
            foreach (CharParty thisparty in MainWindow.CharParties)
            {
                if (thisparty.Count != 0)                                //actually need a placeholder maybe in case no member exists in mixed party
                    thisparty.Name = thisparty[0].PartyName;
            }

            List<MonsterParty> newmonsterList = LoadMonstPartyList(monstpath);
            MainWindow.MonsterParties = newmonsterList;
            foreach (MonsterParty thisparty in MainWindow.MonsterParties)

            {
                if (thisparty.Count != 0)
                    thisparty.Name = thisparty[0].PartyName;
            }
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

                foreach (Fant_Entity charEntity in thisCharParty)
                { thisNewParty.Add(charEntity); }
                foreach (MonsterParty thisMonsterParty in MainWindow.MonsterParties)
                {
                    if (thisMonsterParty.Name == partyNameToCheck)
                    {
                        foreach (Fant_Entity thisMonsterEnt in thisMonsterParty)
                            thisNewParty.Add(thisMonsterEnt);
                    }
                }
            }
            UpdateAllListBoxes();
        }
        private List<CharParty> LoadPartyList(string path)
        {
            MainWindow.CharParties = new List<CharParty>();
            XmlSerializer formatter = new XmlSerializer(MainWindow.CharParties.GetType());
            FileStream aFile = new FileStream(path, FileMode.Open);
            byte[] buffer = new byte[aFile.Length];
            aFile.Read(buffer, 0, (int)aFile.Length);
            MemoryStream stream = new MemoryStream(buffer);
            return (List<CharParty>)formatter.Deserialize(stream);
        }
        private List<MonsterParty> LoadMonstPartyList(string monstpath)
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
                string monstpath = saveFileDialog.FileName;
                char[] MyCharc = { '.', 't', 'x', 't' };
                string newString = path.TrimEnd(MyCharc);
                path = newString + " Char.txt";
                Save(MainWindow.CharParties, path);
                MonstSave(MainWindow.MonsterParties, monstpath);
            }
        }
        private void Save(List<CharParty> partysave, string path)          //saving and loading in XML format at the moment only to allow very fast iteration, will implement an SQL load /save at a later time to show I can do and also for ease of organization if data gets huge
        {
            FileStream outfile = File.Create(path);
            XmlSerializer formatter = new XmlSerializer(partysave.GetType());
            formatter.Serialize(outfile, partysave);
        }
        private void MonstSave(List<MonsterParty> partysave, string monstpath)          //saving and loading in XML format at the moment only to allow very fast iteration, will implement an SQL load /save at a later time to show I can do and also for ease of organization if data gets huge
        {

            FileStream outfile = File.Create(monstpath);
            XmlSerializer formatter = new XmlSerializer(partysave.GetType());
            formatter.Serialize(outfile, partysave);
        }
        #endregion XML Load and Save

        private void QuickCreateObj_Click(object sender, RoutedEventArgs e)
        {
            GlobalItemAdd GlobalItemAdd1 = new GlobalItemAdd();
            GlobalItemAdd1.Show();
        }
        private void DmAdjustChar_Click(object sender, RoutedEventArgs e)
        {
            Fant_Entity entitySel = MainWindow.entitySelected;
            if (entitySel is Character)
            {
                DMUpdateChar DMUpdateChar1 = new DMUpdateChar();
                DMUpdateChar1.Show();
            }
            UpdateAllListBoxes();
        }
        private void CurrentPartyList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdatePartyListBox();
        }
        private void ItemUpdateGlobals_Click(object sender, RoutedEventArgs e)
        {
            UpdateGlobalItems();
        }
        #region combatRounds
        private void GroupsIntoCombat_Click(object sender, RoutedEventArgs e)
        {
            PutTwoGroupsinCombat();
        }
        public void PutTwoGroupsinCombat()
        {
            Party partycombat = (Party)EntGroupList.SelectedItem;
            Party Defparty = (Party)TargetFocusGroupList.SelectedItem;

            foreach (Fant_Entity thisEntity in Defparty)         // might have to change this so you can target own party with spells
                                                                 // alternatively always have own party as valid target may be better
                                                                 // both will eventually have to take into account range
            {
                foreach (Fant_Entity myPartyEntity in Defparty)
                {
                    Target Targetnew = new Target();
                    Targetnew.Name = myPartyEntity.Name;
                    Targetnew.Hp = myPartyEntity.Hp;
                    Targetnew.MaxHp = myPartyEntity.MaxHp;
                    Targetnew.PartyName = myPartyEntity.PartyName;
                    thisEntity.MeleeTargets.Add(Targetnew);
                }
                foreach (Fant_Entity AttEntity in partycombat)
                {
                    Target Targetnew = new Target();
                    Targetnew.Name = AttEntity.Name;
                    Targetnew.Hp = AttEntity.Hp;
                    Targetnew.MaxHp = AttEntity.MaxHp;
                    Targetnew.PartyName = AttEntity.PartyName;
                    thisEntity.MeleeTargets.Add(Targetnew);
                }
            }
            foreach (Fant_Entity thisEntity in partycombat)
            {
                foreach (Fant_Entity DefEntity in Defparty)
                {
                    Target Targetnew = new Target();
                    Targetnew.Name = DefEntity.Name;
                    Targetnew.Hp = DefEntity.Hp;
                    Targetnew.MaxHp = DefEntity.MaxHp;
                    Targetnew.PartyName = DefEntity.PartyName;
                    thisEntity.MeleeTargets.Add(Targetnew);
                }
                foreach (Fant_Entity myPartyEntity in partycombat)
                {
                    Target Targetnew = new Target();
                    Targetnew.Name = myPartyEntity.Name;
                    Targetnew.Hp = myPartyEntity.Hp;
                    Targetnew.MaxHp = myPartyEntity.MaxHp;
                    Targetnew.PartyName = myPartyEntity.PartyName;
                    thisEntity.MeleeTargets.Add(Targetnew);
                }
            }
            foreach (Fant_Entity thisEntity in Defparty)
            {
                Meleegroup.Add(thisEntity);
            }
            foreach (Fant_Entity thisEntity in partycombat)
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
            foreach (Fant_Entity thisEntity in Defparty)
            {
                thisEntity.MeleeTargets.Clear();
                Meleegroup.Remove(thisEntity);
            }
            foreach (Fant_Entity thisEntity in partycombat)
            {
                thisEntity.MeleeTargets.Clear();
                Meleegroup.Remove(thisEntity);
            }
        }
        private void CombatScript(string addToCombatScript)
        {
            StreamWriter comScript;    //   string comScriptPath = @"C:\Users\John MacAulay\Documents\AD&D\JBFantasyGame\combatScript.txt";
                                       // have this string defined in window as will change it g;lobally
            if (!File.Exists(comScriptPath))
            {
                comScript = new StreamWriter(comScriptPath);
            }
            else
            {
                comScript = File.AppendText(comScriptPath);
            }
            comScript.WriteLine(addToCombatScript);
            comScript.WriteLine();
            comScript.Close();
        }
        private void NextCombatRound_Click(object sender, RoutedEventArgs e)
        {
            string timestamp = DateTime.Now.ToString("T");
            CombatScript($"*** New Combat Round *** {timestamp}");
            foreach (Fant_Entity thisEntity in Meleegroup)
            {
                if (thisEntity is Character)
                {
                    Character characterthis = new Character();
                    characterthis = (Character)thisEntity;
                    characterthis.InitRecalc(characterthis);
                    thisEntity.InitMod = characterthis.InitMod;
                }

                RollingDie d60 = new RollingDie(60, 1);
                thisEntity.InitRoll = (d60.Roll() + thisEntity.InitMod);                //can put individual adjustments in at this point later 

            }
            Meleegroup.Sort((x, y) => x.InitRoll.CompareTo(y.InitRoll));   // Awesome this function sorts my list based on the property InitRoll (lowest to highest)
            foreach (Fant_Entity thisEntity in Meleegroup)                      // lower is better on init roll                                                                                // at this point want special abilities to fire, have Duration based and new ones
            {
                if (thisEntity.Hp > 0)
                {
                    CombatScript($"{thisEntity.Name} gets an adjusted initiative roll of  {thisEntity.InitRoll} ");

                    if (thisEntity.MyTargetParty != null)    // so this bit takes care of thisEntity's melee attack is applicable,
                    {
                        string targetParty = thisEntity.MyTargetParty;
                        string targetEntity = thisEntity.MyTargetEnt;
                        foreach (Fant_Entity entitytobeattacked in Meleegroup)
                        {
                            if (entitytobeattacked.PartyName == targetParty && entitytobeattacked.Name == targetEntity)
                            {
                                int Hpb4 = entitytobeattacked.Hp;                          //saving prev HP for a sec
                                if (entitytobeattacked is Character)
                                {
                                    Character attackasCharacter = new Character();
                                    attackasCharacter = (Character)entitytobeattacked;
                                    thisEntity.MeleeAttack(attackasCharacter);
                                }
                                else
                                { thisEntity.MeleeAttack(entitytobeattacked); }

                                int damage = Hpb4 - entitytobeattacked.Hp;
                                CombatScript($"{thisEntity.Name} attacks {entitytobeattacked.Name} for {damage} ");

                                if (entitytobeattacked.Hp < 0)
                                { CombatScript($"{entitytobeattacked.Name} has less than zero hitpoints and is dying."); }
                            }
                        }
                    }
                    foreach (Ability checkAbility in thisEntity.Abilities)
                    {
                        if (checkAbility.AbilIsActive == true)                            //   AbilIsActive means ability activated this round, 
                        {
                            if (thisEntity.CurrentMana >= checkAbility.ManaCost &&
                               thisEntity.ManaRegen >= checkAbility.ManaRegenCost &&
                               thisEntity.Hp >= checkAbility.HpCost)
                            {
                                CombatScript($"{thisEntity.Name} uses {checkAbility.Abil_Name}");
                                thisEntity.CurrentMana -= checkAbility.ManaCost;
                                thisEntity.ManaRegen -= checkAbility.ManaRegenCost;
                                thisEntity.Hp -= checkAbility.HpCost;                          // so takes off costs for ability activated
                                checkAbility.DurationElapsed = 0;                            // this starts the active duration 'timer'                             
                                checkAbility.AbilIsActive = false;
                            }
                            else { CombatScript($"{thisEntity.Name} is unable to use {checkAbility.Abil_Name} "); }
                        }

                        if (checkAbility.DurationMax > checkAbility.DurationElapsed)          // and this bit checks if an ability is still ongoing
                        {
                            if (checkAbility.TargetEntitiesAffected == "Self")                  //put a break in here for self(only) affecting abilities to save cycles
                            {
                                if (checkAbility.Abil_Name == "Regeneration")
                                {
                                    if (thisEntity.Hp < thisEntity.MaxHp)
                                    {
                                        int healing = 0;
                                        string healingRange;
                                        healingRange = checkAbility.HpEffect;                                      // need to allow for two handed etc etc etc 
                                        (int i1, int i2, int i3) = RollingDie.Diecheck(healingRange);
                                        RollingDie thisRoll = new RollingDie(i1, i2, i3);
                                        healing = thisRoll.Roll();
                                        CombatScript($"{thisEntity.Name} regenerates {healing} hit points");
                                        thisEntity.Hp += healing;
                                        if (thisEntity.Hp > thisEntity.MaxHp)
                                        { thisEntity.Hp = thisEntity.MaxHp; }
                                        checkAbility.DurationElapsed += 1;
                                    }
                                }
                            }
                            else
                            {
                                string targetToBeSplit = checkAbility.TargetEntitiesAffected;

                                (string targ1Name, string targ1PartyName, string targ2Name, string targ2PartyName,
                                 string targ3Name, string targ3PartyName, string targ4Name, string targ4PartyName,
                                 string targ5Name, string targ5PartyName, string targ6Name, string targ6PartyName) = TargetSplit(targetToBeSplit);

                                foreach (Fant_Entity entityToBeAffected in Meleegroup)
                                {
                                    bool affectThisEntity = IsEntityAffected(entityToBeAffected, targ1Name, targ1PartyName, targ2Name, targ2PartyName,
                                    targ3Name, targ3PartyName, targ4Name, targ4PartyName, targ5Name, targ5PartyName, targ6Name, targ6PartyName);

                                    if (checkAbility.Abil_Name == "MageThrow" && affectThisEntity == true)
                                    {
                                        RollingDie twentyside = new RollingDie(20, 1);
                                        int tohit;
                                        int assHitOn20;
                                        assHitOn20 = thisEntity.HitOn20 + 10;       // so this give a bonus to hit of plus 10 on normal hit tables; not a guaranteed hit
                                        int attRoll = twentyside.Roll();
                                        if (entityToBeAffected.AC < assHitOn20)
                                        { tohit = 20 - (assHitOn20 - entityToBeAffected.AC); }
                                        else if (entityToBeAffected.AC >= (assHitOn20 + 5))
                                        { tohit = 20 + ((entityToBeAffected.AC - assHitOn20) - 5); }
                                        else tohit = 20;

                                        if (attRoll >= tohit)
                                        {
                                            int damage;
                                            string damagerange;
                                            damagerange = checkAbility.HpEffect;      // an ability will work for 2nd and third attacks per round for characters maybe 
                                            (int i1, int i2, int i3) = RollingDie.Diecheck(damagerange);
                                            RollingDie thisRoll = new RollingDie(i1, i2, i3);
                                            damage = thisRoll.Roll();
                                            CombatScript($"{thisEntity.Name} fires MageThrow at {entityToBeAffected.Name} for {damage} ");
                                            entityToBeAffected.Hp -= damage;
                                        }
                                        checkAbility.DurationElapsed += 1;
                                    }
                                    if (checkAbility.Abil_Name == "HealOverTime" && affectThisEntity == true)
                                    {
                                        int healing = 0;
                                        string healingRange;
                                        healingRange = checkAbility.HpEffect;                                      // need to allow for two handed etc etc etc 
                                        (int i1, int i2, int i3) = RollingDie.Diecheck(healingRange);
                                        RollingDie thisRoll = new RollingDie(i1, i2, i3);
                                        healing = thisRoll.Roll();
                                        CombatScript($"{thisEntity.Name} heals {entityToBeAffected.Name} for {healing} with {checkAbility.Abil_Name}");
                                        entityToBeAffected.Hp += healing;
                                        if (entityToBeAffected.Hp > entityToBeAffected.MaxHp)
                                        { entityToBeAffected.Hp = entityToBeAffected.MaxHp; }
                                        checkAbility.DurationElapsed += 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var combatTxt = File.ReadAllText(comScriptPath);
            CombatDialog.Text = combatTxt;
        }


        public static (string targ1Name, string targ1PartyName, string targ2Name, string targ2PartyName,
            string targ3Name, string targ3PartyName, string targ4Name, string targ4PartyName,
            string targ5Name, string targ5PartyName, string targ6Name, string targ6PartyName) TargetSplit(string targetToBeSplit)
        {
            string targ1Name = "";
            string targ1PartyName = "";
            string targ2Name = "";
            string targ2PartyName = "";
            string targ3Name = "";
            string targ3PartyName = "";
            string targ4Name = "";
            string targ4PartyName = "";
            string targ5Name = "";
            string targ5PartyName = "";
            string targ6Name = "";
            string targ6PartyName = "";
            string[] splitTarget = targetToBeSplit.Split(new Char[] { '|' });
            int countOfSplitTarget = splitTarget.Count();
            if (countOfSplitTarget >= 2)                              //would be nice to write a loop to do this cleanly
            {
                targ1Name = splitTarget[0];
                targ1PartyName = splitTarget[1];
            }
            if (countOfSplitTarget >= 4)
            {
                targ2Name = splitTarget[2];
                targ2PartyName = splitTarget[3];
            }
            if (countOfSplitTarget >= 6)
            {
                targ3Name = splitTarget[4];
                targ3PartyName = splitTarget[5];
            }
            if (countOfSplitTarget >= 8)
            {
                targ4Name = splitTarget[6];
                targ4PartyName = splitTarget[7];
            }
            if (countOfSplitTarget >= 10)
            {
                targ5Name = splitTarget[8];
                targ5PartyName = splitTarget[9];
            }
            if (countOfSplitTarget >= 12)
            {
                targ6Name = splitTarget[10];
                targ6PartyName = splitTarget[11];
            }
            return (targ1Name, targ1PartyName, targ2Name, targ2PartyName, targ3Name, targ3PartyName,
                targ4Name, targ4PartyName, targ5Name, targ5PartyName, targ6Name, targ6PartyName);
        }
        public static bool IsEntityAffected(Fant_Entity entityToBeAffected, string targ1Name, string targ1PartyName, string targ2Name, string targ2PartyName, string targ3Name, string targ3PartyName,
            string targ4Name, string targ4PartyName, string targ5Name, string targ5PartyName, string targ6Name, string targ6PartyName)
        {
            bool isEntityAffected = false;
            if (entityToBeAffected.PartyName == targ1PartyName && entityToBeAffected.Name == targ1Name)
            { isEntityAffected = true; }
            if (entityToBeAffected.PartyName == targ2PartyName && entityToBeAffected.Name == targ2Name)
            { isEntityAffected = true; }
            if (entityToBeAffected.PartyName == targ3PartyName && entityToBeAffected.Name == targ3Name)
            { isEntityAffected = true; }
            if (entityToBeAffected.PartyName == targ4PartyName && entityToBeAffected.Name == targ4Name)
            { isEntityAffected = true; }
            if (entityToBeAffected.PartyName == targ5PartyName && entityToBeAffected.Name == targ5Name)
            { isEntityAffected = true; }
            if (entityToBeAffected.PartyName == targ6PartyName && entityToBeAffected.Name == targ6Name)
            { isEntityAffected = true; }
            return isEntityAffected;
        }

        #endregion combatRounds
        private void MonstGroupList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MonstGroupList.SelectionChanged += MonstGroupList_SelectionChanged;
            UpdateMonstPartyListBox();

        }
        private void MonstCurrentPartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MonstCurrentPartyList.SelectionChanged += MonstCurrentPartyList_SelectionChanged;
            if (MainWindow.entitySelected is Monster)
            {
                MainWindow.entitySelected = (Monster)CurrentPartyList.SelectedItem;
                UpdateMonstPartyListBox();
            }
        }
        private void MonstCurrentPartyList_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            UpdateMonstPartyListBox();
        }
        private void EntCurrentPartyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EntCurrentPartyList.SelectionChanged += EntCurrentPartyList_SelectionChanged;
            MainWindow.entitySelected = (Fant_Entity)EntCurrentPartyList.SelectedItem;
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
            Fant_Entity selected = MainWindow.entitySelected; ;
            string removeFromGroup = selected.PartyName;
            if (selected is Character)
            {
                foreach (CharParty thisCharParty in MainWindow.CharParties)
                    if (thisCharParty.Name == removeFromGroup)
                    { thisCharParty.Remove((Character)selected); }
            }
            if (selected is Monster)
            {
                foreach (MonsterParty thisMonstParty in MainWindow.MonsterParties)
                    if (thisMonstParty.Name == removeFromGroup)
                    { thisMonstParty.Remove((Monster)selected); }
            }
            foreach (Party thisEntParty in MainWindow.Parties)
                if (thisEntParty.Name == removeFromGroup)
                { thisEntParty.Remove(selected); }

            UpdateAllListBoxes();

        }
        #region SQL Save and Load
        public void UpdateSQLList()
        {
            Fant_Entities = new ObservableCollection<Fant_Entity>      // this is a shortened Fant_Entity (type) list to save memory
            { };
            con.Open();
            SqlCommand cmdthis;
            SqlDataReader dataReader;
            string sql;                        //  Output = "";    don't care about Ouput from here

            sql = "select Name, PartyName,Lvl from Fant_Character";
            cmdthis = new SqlCommand(sql, con);
            dataReader = cmdthis.ExecuteReader();
            while (dataReader.Read())
            {
                Character addCharacter = new Character();
                addCharacter.Name = (string)dataReader.GetValue(0);
                addCharacter.PartyName = (string)dataReader.GetValue(1);
                addCharacter.Lvl = dataReader.GetByte(2);             //  this is using an implict cast Byte to Int
                Fant_Entities.Add(addCharacter);
            }
            con.Close();
            con.Open();
            sql = "select Name, PartyName,Lvl from monster";
            cmdthis = new SqlCommand(sql, con);
            dataReader = cmdthis.ExecuteReader();
            while (dataReader.Read())
            {
                Monster addMonster = new Monster();
                addMonster.Name = (string)dataReader.GetValue(0);
                addMonster.PartyName = (string)dataReader.GetValue(1);
                addMonster.Lvl = dataReader.GetByte(2);
                Fant_Entities.Add(addMonster);
            }

            Fant_Ents_inSQL.ItemsSource = Fant_Entities;
            con.Close();


        }
        public ObservableCollection<Fant_Entity> Fant_Entities
        {
            get { return (ObservableCollection<Fant_Entity>)GetValue(Fant_EntityProperty); }
            set { SetValue(Fant_EntityProperty, value); }
        }
        public static readonly DependencyProperty Fant_EntityProperty =
            DependencyProperty.Register("Fant_Entities",
                     typeof(ObservableCollection<Fant_Entity>),
                     typeof(JBFantasyGame.DMMainWin),
                     new PropertyMetadata(null));

        private void SQLSave_Click(object sender, RoutedEventArgs e)
        {

            Fant_Entity selected = MainWindow.entitySelected;
            bool entExists = DoesEntExistInSQL(selected);
            if (entExists == true)
            { SQLUpdateFantEntity(selected); }
            else if (entExists == false)
            { SQLSaveFantEntity(selected); }
        }
        private void SQLLoad_Click(object sender, RoutedEventArgs e)
        {
            Fant_Entity entThis = new Fant_Entity();
            entThis = (Fant_Entity)Fant_Ents_inSQL.SelectedItem;
            SQLLoadEnt(entThis);
        }

        private void SQLLoadEnt(Fant_Entity entThis)
        {
            if (entThis.Name != null)
            {
                //MessageBox.Show($"I am about to load from SQL for {entThis.Name }");//stub for loading from SQL Database; actually I think I will make a reader and display first 
                // var (fantExists, isChar) =  ExistinCurrentLists(entThis);            as I always want this to upload from SQl as Definitive
                ///  if (fantExists==false)   
                // {
                Monster UpLoadedMonst = new Monster();
                Character UpLoadedChar = new Character();
                con.Open();
                SqlCommand cmdPartyUpload;
                SqlDataReader dataReader;
                string sql;               //, Output="";
                sql = "select AC, HitOn20, Hp, InitMod, InitRoll, IsAlive, Lvl, MaxHp, MyTargetEnt, MyTargetParty, MyTurn," +
                    "Name, PartyName,CharType,Chr,Con,Dex, Exp, Inte, Str, Wis, MaxMana, CurrentMana, MaxManaRegen,ManaRegen," +
                    "XPOnDefeat, DefeatMult from Fant_Character ";                      //add the rest here
                sql = sql + $"where PartyName = '{entThis.PartyName}' and Name = '{entThis.Name}' ";
                cmdPartyUpload = new SqlCommand(sql, con);

                dataReader = cmdPartyUpload.ExecuteReader();
                while (dataReader.Read())
                {
                    UpLoadedChar.AC = dataReader.GetByte(0);
                    UpLoadedChar.HitOn20 = dataReader.GetByte(1);
                    UpLoadedChar.Hp = dataReader.GetInt16(2);
                    UpLoadedChar.InitMod = dataReader.GetInt16(3);
                    UpLoadedChar.InitRoll = dataReader.GetByte(4);
                    UpLoadedChar.IsAlive = dataReader.GetBoolean(5);
                    UpLoadedChar.Lvl = dataReader.GetByte(6);
                    UpLoadedChar.MaxHp = dataReader.GetInt16(7);
                    UpLoadedChar.MyTargetEnt = dataReader.GetString(8);
                    UpLoadedChar.MyTargetParty = dataReader.GetString(9);
                    UpLoadedChar.MyTurn = dataReader.GetBoolean(10);
                    UpLoadedChar.Name = dataReader.GetString(11);
                    UpLoadedChar.PartyName = dataReader.GetString(12);
                    UpLoadedChar.CharType = dataReader.GetString(13);
                    UpLoadedChar.Chr = dataReader.GetByte(14);
                    UpLoadedChar.Con = dataReader.GetByte(15);
                    UpLoadedChar.Dex = dataReader.GetByte(16);
                    UpLoadedChar.Exp = (int)dataReader.GetInt64(17);
                    UpLoadedChar.Inte = dataReader.GetByte(18);
                    UpLoadedChar.Str = dataReader.GetByte(19);
                    UpLoadedChar.Wis = dataReader.GetByte(20);

                    UpLoadedChar.MaxMana = dataReader.GetDouble(21);
                    UpLoadedChar.CurrentMana = dataReader.GetDouble(22);
                    UpLoadedChar.MaxManaRegen = dataReader.GetDouble(23);
                    UpLoadedChar.ManaRegen = dataReader.GetDouble(24);
                    UpLoadedChar.XPOnDefeat = dataReader.GetDouble(25);
                    UpLoadedChar.DefeatMult = dataReader.GetDouble(26);
                    // ok this bit seems to have sucessfully loaded basic character need to add inventory items
                }
                con.Close();
                con.Open();
                SqlCommand cmdPartyUpload4;
                SqlDataReader dataReader4;
                string sql4;               //, Output="";
                sql4 = "select AC, HitOn20, Hp, InitMod, InitRoll, IsAlive, Lvl, MaxHp, MyTargetEnt, MyTargetParty, MyTurn," +
                    "Name, PartyName,MonsterType, NoOfAtt, DamPerAtt1, DamPerAtt2, DamPerAtt3, HitDie, MaxMana, CurrentMana," +
                    " MaxManaRegen,ManaRegen, XPOnDefeat, DefeatMult from Monster ";                      //add the rest here
                sql4 = sql4 + $"where PartyName = '{entThis.PartyName}' and Name = '{entThis.Name}' ";
                cmdPartyUpload4 = new SqlCommand(sql4, con);

                dataReader4 = cmdPartyUpload4.ExecuteReader();
                while (dataReader4.Read())
                {
                    UpLoadedMonst.AC = dataReader4.GetByte(0);
                    UpLoadedMonst.HitOn20 = dataReader4.GetByte(1);
                    UpLoadedMonst.Hp = dataReader4.GetInt16(2);
                    UpLoadedMonst.InitMod = dataReader4.GetInt16(3);
                    UpLoadedMonst.InitRoll = dataReader4.GetByte(4);
                    UpLoadedMonst.IsAlive = dataReader4.GetBoolean(5);
                    UpLoadedMonst.Lvl = dataReader4.GetByte(6);
                    UpLoadedMonst.MaxHp = dataReader4.GetInt16(7);
                    UpLoadedMonst.MyTargetEnt = dataReader4.GetString(8);
                    UpLoadedMonst.MyTargetParty = dataReader4.GetString(9);
                    UpLoadedMonst.MyTurn = dataReader4.GetBoolean(10);
                    UpLoadedMonst.Name = dataReader4.GetString(11);
                    UpLoadedMonst.PartyName = dataReader4.GetString(12);
                    UpLoadedMonst.MonsterType = dataReader4.GetString(13);
                    UpLoadedMonst.NoOfAtt = dataReader4.GetByte(14);
                    UpLoadedMonst.DamPerAtt1 = dataReader4.GetString(15);
                    UpLoadedMonst.DamPerAtt2 = dataReader4.GetString(16);
                    UpLoadedMonst.DamPerAtt3 = dataReader4.GetString(17);
                    UpLoadedMonst.HitDie = dataReader4.GetString(18);
                    UpLoadedMonst.MaxMana = dataReader4.GetDouble(19);
                    UpLoadedMonst.CurrentMana = dataReader4.GetDouble(20);
                    UpLoadedMonst.MaxManaRegen = dataReader4.GetDouble(21);
                    UpLoadedMonst.ManaRegen = dataReader4.GetDouble(22);
                    UpLoadedMonst.XPOnDefeat = dataReader4.GetDouble(23);
                    UpLoadedMonst.DefeatMult = dataReader4.GetDouble(24);
                }
                con.Close();
                con.Open();
                SqlCommand cmdPartyUpload2;
                SqlDataReader dataReader2;
                string sql2;                      //, Output2 = "";
                sql2 = "select ACEffect, Damage, IsEquipped, Name, ObjType, DescrPhysObj " +
                      " from PhysObj ";                      //add the rest here
                sql2 = sql2 + $"where OwnersPartyName = '{entThis.PartyName}' and OwnersName = '{entThis.Name}' ";
                cmdPartyUpload2 = new SqlCommand(sql2, con);
                dataReader2 = cmdPartyUpload2.ExecuteReader();
                while (dataReader2.Read())
                {
                    PhysObj addPhysObj = new PhysObj();
                    addPhysObj.ACEffect = dataReader2.GetByte(0);
                    addPhysObj.Damage = dataReader2.GetString(1);
                    addPhysObj.IsEquipped = dataReader2.GetBoolean(2);
                    addPhysObj.Name = dataReader2.GetString(3);
                    addPhysObj.ObjType = dataReader2.GetString(4);
                    addPhysObj.DescrPhysObj = dataReader2.GetString(5);
                    if (UpLoadedMonst.Name != null)
                    { UpLoadedMonst.Inventory.Add(addPhysObj); }
                    if (UpLoadedChar.Name != null)
                    { UpLoadedChar.Inventory.Add(addPhysObj); }
                }
                con.Close();

                con.Open();
                SqlCommand cmdPartyUpload5;
                SqlDataReader dataReader5;
                string sql5;
                sql5 = "select Abil_Name, Abil_Level, DescrOfAbility, AbilIsActive, ManaCost, ManaRegenCost, HpCost, DurationMax, DurationElapsed," +
                    " NoOfEntitiesAffectedMax, TargetEntitiesAffected, HpEffect, TargetRange, SaveType" +
                    " from Ability ";
                sql5 = sql5 + $"where OwnersPartyName = '{entThis.PartyName}' and OwnersName = '{entThis.Name}' ";
                cmdPartyUpload5 = new SqlCommand(sql5, con);
                dataReader5 = cmdPartyUpload5.ExecuteReader();
                while (dataReader5.Read())
                {
                    Ability addAbility = new Ability();
                    addAbility.Abil_Name = dataReader5.GetString(0);
                    addAbility.Abil_Level = dataReader5.GetByte(1);
                    addAbility.DescrOfAbility = dataReader5.GetString(2);
                    addAbility.AbilIsActive = dataReader5.GetBoolean(3);
                    addAbility.ManaCost = dataReader5.GetDouble(4);
                    addAbility.ManaRegenCost = dataReader5.GetDouble(5);
                    addAbility.HpCost = dataReader5.GetInt16(6);
                    addAbility.DurationMax = dataReader5.GetInt16(7);
                    addAbility.DurationElapsed = dataReader5.GetInt16(8);
                    addAbility.NoOfEntitiesAffectedMax = dataReader5.GetByte(9);
                    addAbility.TargetEntitiesAffected = dataReader5.GetString(10);
                    addAbility.HpEffect = dataReader5.GetString(11);
                    addAbility.TargetRange = dataReader5.GetDouble(12);
                    addAbility.SaveType = dataReader5.GetString(13);

                    if (UpLoadedMonst.Name != null)
                    { UpLoadedMonst.Abilities.Add(addAbility); }
                    if (UpLoadedChar.Name != null)
                    { UpLoadedChar.Abilities.Add(addAbility); }
                }


                con.Close();
                con.Open();
                SqlCommand cmdPartyUpload3;
                SqlDataReader dataReader3;
                string sql3;                      //, Output2 = "";
                sql3 = "select IsAlive, Name, Hp, MaxHp, PartyName " +
                      " from Target ";                      //add the rest here
                sql3 = sql3 + $"where OwnersPartyName = '{entThis.PartyName}' and OwnersName = '{entThis.Name}' ";
                cmdPartyUpload3 = new SqlCommand(sql3, con);
                dataReader3 = cmdPartyUpload3.ExecuteReader();
                while (dataReader3.Read())
                {
                    Target addTarget = new Target();
                    addTarget.IsAlive = dataReader3.GetBoolean(0);
                    addTarget.Name = dataReader3.GetString(1);
                    addTarget.Hp = dataReader3.GetInt16(2);
                    addTarget.MaxHp = dataReader3.GetInt16(3);
                    addTarget.PartyName = dataReader3.GetString(4);
                    if (UpLoadedMonst.Name != null)
                    { UpLoadedMonst.MeleeTargets.Add(addTarget); }
                    if (UpLoadedChar.Name != null)
                    { UpLoadedChar.MeleeTargets.Add(addTarget); }
                }
                con.Close();

                bool entPartyExist = false;
                foreach (Party checkParty in MainWindow.Parties)
                {
                    if (checkParty.Name == entThis.PartyName)
                    { entPartyExist = true; }
                }
                if (entPartyExist == false)          // only checking EntityParty List as the other two should only exist in tandem with this one
                {
                    Party newEntParty = new Party();
                    CharParty newCharParty = new CharParty();
                    MonsterParty newMonsterParty = new MonsterParty();
                    newEntParty.Name = entThis.PartyName;
                    newCharParty.Name = entThis.PartyName;
                    newMonsterParty.Name = entThis.PartyName;
                    if (UpLoadedChar.Name != null)
                    {
                        newEntParty.Add(UpLoadedChar);
                        newCharParty.Add(UpLoadedChar);
                    }
                    if (UpLoadedMonst.Name != null)
                    {
                        newEntParty.Add(UpLoadedMonst);
                        newMonsterParty.Add(UpLoadedMonst);
                    }
                    MainWindow.Parties.Add(newEntParty);
                    MainWindow.CharParties.Add(newCharParty);
                    MainWindow.MonsterParties.Add(newMonsterParty);
                }
                if (entPartyExist == true && UpLoadedChar.Name != null)
                {
                    foreach (Party addToThisParty in MainWindow.Parties)
                    {
                        if (addToThisParty.Name == entThis.PartyName)            // need to have something here to check if character is already here
                        {
                            int index = 0;
                            bool checkEntExists = false;
                            foreach (Fant_Entity check_Ent in addToThisParty)
                            {
                                if (check_Ent.Name == entThis.Name)
                                {
                                    checkEntExists = true;
                                    index = addToThisParty.IndexOf(check_Ent);
                                }
                            }
                            if (checkEntExists == false)
                            { addToThisParty.Add(UpLoadedChar); }
                            if (checkEntExists == true)
                            { addToThisParty[index] = UpLoadedChar; }
                        }
                    }
                    foreach (CharParty addToCharParty in MainWindow.CharParties)
                    {
                        if (addToCharParty.Name == entThis.PartyName)
                        {
                            int index = 0;
                            bool checkCharExists = false;
                            foreach (Character check_Char in addToCharParty)
                            {
                                if (check_Char.Name == entThis.Name)
                                {
                                    checkCharExists = true;
                                    index = addToCharParty.IndexOf(check_Char);
                                }
                            }
                            if (checkCharExists == false)
                            { addToCharParty.Add(UpLoadedChar); }
                            if (checkCharExists == true)
                            { addToCharParty[index] = UpLoadedChar; }
                        }
                    }
                }
                if (entPartyExist == true && UpLoadedMonst.Name != null)
                {
                    foreach (Party addToThisParty in MainWindow.Parties)
                    {
                        if (addToThisParty.Name == entThis.PartyName)
                        {
                            int index = 0;
                            bool checkEntExists = false;
                            foreach (Fant_Entity check_Ent in addToThisParty)
                            {
                                if (check_Ent.Name == entThis.Name)
                                {
                                    checkEntExists = true;
                                    index = addToThisParty.IndexOf(check_Ent);
                                }
                            }
                            if (checkEntExists == false)
                            { addToThisParty.Add(UpLoadedMonst); }
                            if (checkEntExists == true)
                            { addToThisParty[index] = UpLoadedMonst; }
                        }
                    }
                    foreach (MonsterParty addToMonstParty in MainWindow.MonsterParties)
                    {
                        if (addToMonstParty.Name == entThis.PartyName)
                        {
                            int index = 0;
                            bool checkMonstExists = false;
                            foreach (Monster checkMonst in addToMonstParty)
                            {
                                if (checkMonst.Name == entThis.Name)
                                {
                                    checkMonstExists = true;
                                    index = addToMonstParty.IndexOf(checkMonst);
                                }
                            }
                            if (checkMonstExists == false)
                            { addToMonstParty.Add(UpLoadedMonst); }
                            if (checkMonstExists == true)
                            { addToMonstParty[index] = UpLoadedMonst; }
                        }
                    }
                }

            }
            UpdateAllListBoxes();
        }
        private static (bool Fant_exists, bool isChar) ExistinCurrentLists(Fant_Entity checkThisOne)
        {
            bool isChar = false;
            bool Fant_Exists = false;
            foreach (CharParty listCheckParty in MainWindow.CharParties)
            {
                CharParty CheckParty = listCheckParty;
                foreach (Character checkChar in CheckParty)
                {
                    if (checkChar.Name == checkThisOne.Name && checkChar.PartyName == checkThisOne.PartyName)
                    {
                        Fant_Exists = true;
                        isChar = true;
                    }
                }
            }
            foreach (MonsterParty listCheckParty in MainWindow.MonsterParties)
            {
                MonsterParty checkParty = listCheckParty;
                foreach (Monster checkMonst in checkParty)
                    if (checkMonst.Name == checkThisOne.Name && checkMonst.PartyName == checkThisOne.PartyName)
                    {
                        Fant_Exists = true;
                    }
            }
            return (Fant_Exists, isChar);
        }
        private void ExistinSQL_Click(object sender, RoutedEventArgs e)
        {
            Fant_Entity selected = MainWindow.entitySelected;
            bool entExists = DoesEntExistInSQL(selected);
            if (entExists == true)
            {
                MessageBox.Show($"{selected.Name} in {selected.PartyName} already exists in the database; " +
                  $"I think you meant to update it.");
                con.Open();
                SqlCommand updateSQlFile = new SqlCommand("update Fant_Character set MyTurn=0 " +
                   "where Name = @selectedName and PartyName= @selectedPartyName", con);
                updateSQlFile.Parameters.AddWithValue("@selectedName", selected.Name);
                updateSQlFile.Parameters.AddWithValue("@selectedPartyName", selected.PartyName);

                updateSQlFile.ExecuteNonQuery();
                con.Close();
            }
        }
        private bool DoesPartyExistinSQL(string checkPartyName)
        {
            bool monstPartyExists = false;
            bool partyExists = false;
            con.Open();
            SqlCommand cmdPartyCheck;
            SqlDataReader dataReader;
            string sql;
            sql = "select PartyName from Fant_Character";
            cmdPartyCheck = new SqlCommand(sql, con);
            dataReader = cmdPartyCheck.ExecuteReader();
            while (dataReader.Read())
            {
                if ((string)dataReader.GetValue(0) == checkPartyName)
                {
                    partyExists = true;
                }
            }
            con.Close();
            if (partyExists == true)
            { MessageBox.Show($"{checkPartyName} already exists in this Database, with Character members."); }

            con.Open();
            SqlCommand cmdMonstPartyCheck;
            SqlDataReader dataReaderMonst;
            string sqlMonst;
            sqlMonst = "select partyName from Monster";
            cmdMonstPartyCheck = new SqlCommand(sqlMonst, con);
            dataReaderMonst = cmdMonstPartyCheck.ExecuteReader();
            while (dataReaderMonst.Read())
            {
                if ((string)dataReaderMonst.GetValue(0) == checkPartyName)
                {
                    monstPartyExists = true;
                    partyExists = true;
                }
            }
            con.Close();
            if (monstPartyExists == true)
            { MessageBox.Show($"{checkPartyName} already exists in this Database, with Monster members."); }
            return partyExists;
        }

        public bool DoesEntExistInSQL(Fant_Entity selected)
        {
            bool entExists = false;
            string entPartyNameToCheck = selected.PartyName;
            string entNameToCheck = selected.Name;
            con.Open();
            SqlCommand cmdthis;
            SqlDataReader dataReader;
            string sql, Output = "";
            // if (selected is Character)
            sql = "select Name, PartyName from Fant_Character";        //should default to this, change on Monster 
            if (selected is Monster)
            { sql = "select Name, PartyName from monster"; }
            cmdthis = new SqlCommand(sql, con);
            dataReader = cmdthis.ExecuteReader();
            while (dataReader.Read())
            {
                Output = Output + dataReader.GetValue(0) + "-" + dataReader.GetValue(1) + "\n";
                if (entPartyNameToCheck == (string)dataReader.GetValue(1) && entNameToCheck == (string)dataReader.GetValue(0))
                { entExists = true; }
            }
            // MessageBox.Show(Output);
            con.Close();
            return entExists;
        }
        private void SQLUpdateFantEntity(Fant_Entity selected)
        {
            con.Open();
            // for the inventory and target items, even for update process I think it will be safest to clear all owned items
            // and targets from SQL DB and then reinsert them from Fant_entity in question
            SqlCommand cmdDelObj = new SqlCommand("Delete from PhysObj " +
                       "where OwnersName = @selectedName and OwnersPartyName= @selectedPartyName", con);    //deletes this Fant_Ent's items
            cmdDelObj.Parameters.AddWithValue("@selectedName", selected.Name);
            cmdDelObj.Parameters.AddWithValue("@selectedPartyName", selected.PartyName);
            cmdDelObj.ExecuteNonQuery();
            con.Close();
            InsertObjInSQLInv(selected);         //inserts this Fant_Ent's Items

            con.Open();
            SqlCommand cmdDelTarg = new SqlCommand("Delete from Target " +
                       "where OwnersName = @selectedName and OwnersPartyName= @selectedPartyName", con);
            cmdDelTarg.Parameters.AddWithValue("@selectedName", selected.Name);
            cmdDelTarg.Parameters.AddWithValue("@selectedPartyName", selected.PartyName);
            cmdDelTarg.ExecuteNonQuery();
            con.Close();
            InsertTargetsSQLTargets(selected);      //inserts this Fant_Ent's Targets

            con.Open();
            SqlCommand cmdDelAbil = new SqlCommand("Delete from Ability " +
                    "where OwnersName = @selectedName and OwnersPartyName= @selectedPartyName", con);    //deletes this Fant_Ent's Abilities
            cmdDelAbil.Parameters.AddWithValue("@selectedName", selected.Name);
            cmdDelAbil.Parameters.AddWithValue("@selectedPartyName", selected.PartyName);
            cmdDelAbil.ExecuteNonQuery();
            con.Close();
            InsertAbilitiesSQLAbility(selected);

            if (selected is Character)
            {
                con.Open();
                Character charSelected = (Character)selected;
                //selected.MyTurn = true;    //temp just to see the issue
                SqlCommand cmd2 = new SqlCommand("update Fant_Character set AC = @AC, HitOn20 =@HitOn20, Hp=@Hp, InitMod =@InitMod," +
                    " InitRoll=@InitRoll, IsAlive=@IsAlive, Lvl=@Lvl, MaxHp=@MaxHp, MyTurn=@MyTurn, " +
                    "CharType=@CharType, Chr=@Chr, Con=@Con, Dex=@Dex, Exp=@Exp, Inte=@Inte, Str=@Str, Wis=@Wis," +
                    "MyTargetEnt=@MyTargetEnt, MyTargetParty=@MyTargetParty," +
                    "MaxMana=@MaxMana, CurrentMana=@CurrentMana, MaxManaRegen=@MaxManaRegen, ManaRegen=@ManaRegen," +
                    "XPOnDefeat=@XPOnDefeat, DefeatMult=@DefeatMult " +
                    "where Name = @Name and PartyName= @PartyName", con);     //taken out for now    @MyTargetEnt, @MyTargetParty, 
                cmd2.Parameters.AddWithValue("@AC", selected.AC);
                cmd2.Parameters.AddWithValue("@HitOn20", selected.HitOn20);
                cmd2.Parameters.AddWithValue("@Hp", selected.Hp);
                cmd2.Parameters.AddWithValue("@InitMod", selected.InitMod);
                cmd2.Parameters.AddWithValue("@InitRoll", selected.InitRoll);
                int? CharIsAlive = null;
                if (selected.IsAlive == true)
                { CharIsAlive = 1; }
                if (selected.IsAlive == false)
                { CharIsAlive = 0; }
                cmd2.Parameters.AddWithValue("@IsAlive", CharIsAlive);
                cmd2.Parameters.AddWithValue("@Lvl", selected.Lvl);
                cmd2.Parameters.AddWithValue("@MaxHp", selected.MaxHp);
                int? CharMyTurn = null;
                if (selected.MyTurn == true)
                { CharMyTurn = 1; }
                if (selected.MyTurn == false)
                { CharMyTurn = 0; }
                cmd2.Parameters.AddWithValue("@MyTurn", CharMyTurn);
                cmd2.Parameters.AddWithValue("@Name", selected.Name);
                cmd2.Parameters.AddWithValue("@PartyName", selected.PartyName);
                //new unique values for character
                cmd2.Parameters.AddWithValue("@CharType", charSelected.CharType);
                cmd2.Parameters.AddWithValue("@Chr", charSelected.Chr);
                cmd2.Parameters.AddWithValue("@Con", charSelected.Con);
                cmd2.Parameters.AddWithValue("@Dex", charSelected.Dex);
                cmd2.Parameters.AddWithValue("@Exp", charSelected.Exp);
                cmd2.Parameters.AddWithValue("@Inte", charSelected.Inte);
                cmd2.Parameters.AddWithValue("@Str", charSelected.Str);
                cmd2.Parameters.AddWithValue("@Wis", charSelected.Wis);
                cmd2.Parameters.AddWithValue("@MaxMana", charSelected.MaxMana);
                cmd2.Parameters.AddWithValue("@CurrentMana", charSelected.CurrentMana);
                cmd2.Parameters.AddWithValue("@MaxManaRegen", charSelected.MaxManaRegen);
                cmd2.Parameters.AddWithValue("@ManaRegen", charSelected.ManaRegen);
                cmd2.Parameters.AddWithValue("@XPOnDefeat", charSelected.XPOnDefeat);
                cmd2.Parameters.AddWithValue("@DefeatMult", charSelected.DefeatMult);
                if (selected.MyTargetEnt != null)
                {
                    cmd2.Parameters.AddWithValue("@MyTargetEnt", selected.MyTargetEnt);
                    cmd2.Parameters.AddWithValue("@MyTargetParty", selected.MyTargetParty);
                }
                else
                {
                    cmd2.Parameters.AddWithValue("@MyTargetEnt", "");
                    cmd2.Parameters.AddWithValue("@MyTargetParty", "");
                }
                cmd2.ExecuteNonQuery();
                con.Close();
            }
            if (selected is Monster)
            {
                con.Open();
                Monster monstSelected = (Monster)selected;
                SqlCommand cmd3 = new SqlCommand("update Monster set AC=@AC, MonsterType =@MonsterType, HitOn20=@HitOn20," +
                    " Hp=@Hp, InitMod=@InitMod, InitRoll=@InitRoll, IsAlive=@IsAlive," +
                    " MaxHp=@MaxHp, MyTargetEnt=@MyTargetEnt, MyTargetParty=@MyTargetParty, MyTurn=@MyTurn, Lvl=@Lvl," +
                    " NoOfAtt=@NoOfAtt, DamPerAtt1=@DamPerAtt1, DamPerAtt2=@DamPerAtt2, DamPerAtt3=@DamPerAtt3, HitDie=@HitDie," +
                    "MaxMana=@MaxMana, CurrentMana=@CurrentMana, MaxManaRegen=@MaxManaRegen, ManaRegen=@ManaRegen," +
                    "XPOnDefeat=@XPOnDefeat, DefeatMult=@DefeatMult" +
                    " where Name = @Name and PartyName = @PartyName", con);
                cmd3.Parameters.AddWithValue("@AC", monstSelected.AC);
                cmd3.Parameters.AddWithValue("@MonsterType", monstSelected.MonsterType);
                cmd3.Parameters.AddWithValue("@HitOn20", monstSelected.HitOn20);
                cmd3.Parameters.AddWithValue("@Hp", monstSelected.Hp);
                cmd3.Parameters.AddWithValue("@InitMod", monstSelected.InitMod);
                cmd3.Parameters.AddWithValue("@InitRoll", monstSelected.InitRoll);
                cmd3.Parameters.AddWithValue("@IsAlive", monstSelected.IsAlive);
                cmd3.Parameters.AddWithValue("@MaxHp", monstSelected.MaxHp);
                if (monstSelected.MyTargetEnt != null)
                {
                    cmd3.Parameters.AddWithValue("@MyTargetEnt", monstSelected.MyTargetEnt);
                    cmd3.Parameters.AddWithValue("@MyTargetParty", monstSelected.MyTargetParty);
                }
                else
                {
                    cmd3.Parameters.AddWithValue("@MyTargetEnt", "");
                    cmd3.Parameters.AddWithValue("@MyTargetParty", "");
                }
                cmd3.Parameters.AddWithValue("@MyTurn", monstSelected.MyTurn);
                cmd3.Parameters.AddWithValue("@Name", monstSelected.Name);
                cmd3.Parameters.AddWithValue("@PartyName", monstSelected.PartyName);
                cmd3.Parameters.AddWithValue("@Lvl", monstSelected.Lvl);
                cmd3.Parameters.AddWithValue("@NoOfAtt", monstSelected.NoOfAtt);
                cmd3.Parameters.AddWithValue("@DamPerAtt1", monstSelected.DamPerAtt1);
                cmd3.Parameters.AddWithValue("@DamPerAtt2", monstSelected.DamPerAtt2);
                cmd3.Parameters.AddWithValue("@DamPerAtt3", monstSelected.DamPerAtt3);
                cmd3.Parameters.AddWithValue("@HitDie", monstSelected.HitDie);
                cmd3.Parameters.AddWithValue("@MaxMana", monstSelected.MaxMana);
                cmd3.Parameters.AddWithValue("@CurrentMana", monstSelected.CurrentMana);
                cmd3.Parameters.AddWithValue("@MaxManaRegen", monstSelected.MaxManaRegen);
                cmd3.Parameters.AddWithValue("@ManaRegen", monstSelected.ManaRegen);
                cmd3.Parameters.AddWithValue("@XPOnDefeat", monstSelected.XPOnDefeat);
                cmd3.Parameters.AddWithValue("@DefeatMult", monstSelected.DefeatMult);
                cmd3.ExecuteNonQuery();
                con.Close();
            }
        }
        private void InsertTargetsSQLTargets(Fant_Entity selected)
        {
            con.Open();
            foreach (Target thisTarget in selected.MeleeTargets)
            {
                SqlCommand cmdTarget = new SqlCommand("Insert into Target (OwnersPartyName,OwnersName,IsAlive,Name,PartyName,Hp,MaxHp )" +
                    " values (@OwnersPartyName,@OwnersName,@IsAlive,@Name,@PartyName,@Hp,@MaxHp)", con);
                cmdTarget.Parameters.AddWithValue("@OwnersPartyName", selected.PartyName);
                cmdTarget.Parameters.AddWithValue("@OwnersName", selected.Name);
                int? targetIsAlive = null;
                if (thisTarget.IsAlive == true)
                { targetIsAlive = 1; }
                if (thisTarget.IsAlive == false)
                { targetIsAlive = 0; }
                cmdTarget.Parameters.AddWithValue("@IsAlive", targetIsAlive);
                cmdTarget.Parameters.AddWithValue("@Name", thisTarget.Name);
                cmdTarget.Parameters.AddWithValue("@PartyName", thisTarget.PartyName);
                cmdTarget.Parameters.AddWithValue("@Hp", thisTarget.Hp);
                cmdTarget.Parameters.AddWithValue("@MaxHp", thisTarget.MaxHp);
                cmdTarget.ExecuteNonQuery();
            }
            con.Close();
        }
        private void InsertObjInSQLInv(Fant_Entity selected)
        {
            foreach (PhysObj thisPhysObj in selected.Inventory)
            {
                con.Open();
                SqlCommand cmdObj = new SqlCommand("Insert into PhysObj (OwnersPartyName,OwnersName,ACEffect,Damage,IsEquipped,Name,ObjType,DescrPhysObj) " +
                    "values  (@OwnersPartyName, @OwnersName, @ACEffect, @Damage, @IsEquipped, @Name, @ObjType, @DescrPhysObj)", con);
                cmdObj.Parameters.AddWithValue("@OwnersPartyName", selected.PartyName);
                cmdObj.Parameters.AddWithValue("@OwnersName", selected.Name);
                cmdObj.Parameters.AddWithValue("@ACEffect", thisPhysObj.ACEffect);
                cmdObj.Parameters.AddWithValue("@Damage", thisPhysObj.Damage);
                int? isEquipped = null;
                if (thisPhysObj.IsEquipped == true)
                { isEquipped = 1; }
                if (thisPhysObj.IsEquipped == false)
                { isEquipped = 0; }
                cmdObj.Parameters.AddWithValue("IsEquipped", isEquipped);
                cmdObj.Parameters.AddWithValue("@Name", thisPhysObj.Name);
                cmdObj.Parameters.AddWithValue("@ObjType", thisPhysObj.ObjType);
                cmdObj.Parameters.AddWithValue("@DescrPhysObj", thisPhysObj.DescrPhysObj);
                cmdObj.ExecuteNonQuery();
                con.Close();
            }
        }
        private void InsertAbilitiesSQLAbility(Fant_Entity selected)
        {
            foreach (Ability thisAbility in selected.Abilities)
            {
                con.Open();
                SqlCommand cmdAbil = new SqlCommand("Insert into Ability (OwnersPartyName, OwnersName, Abil_Name, Abil_Level, DescrOfAbility," +
                    " AbilIsActive, ManaCost, ManaRegenCost, HpCost, DurationMax, DurationElapsed, NoOfEntitiesAffectedMax," +
                    " TargetEntitiesAffected, HpEffect, TargetRange, SaveType" +
                    ") values (@OwnersPartyName, @OwnersName, @Abil_Name, @Abil_Level, @DescrOfAbility, " +
                    " @AbilIsActive, @ManaCost, @ManaRegenCost, @HpCost, @DurationMax, @DurationElapsed, @NoOfEntitiesAffectedMax," +
                    " @TargetEntitiesAffected, @HpEffect, @TargetRange, @SaveType)", con);
                cmdAbil.Parameters.AddWithValue("@OwnersPartyName", selected.PartyName);
                cmdAbil.Parameters.AddWithValue("@OwnersName", selected.Name);
                cmdAbil.Parameters.AddWithValue("@Abil_Name", thisAbility.Abil_Name);
                cmdAbil.Parameters.AddWithValue("@Abil_Level", thisAbility.Abil_Level);
                cmdAbil.Parameters.AddWithValue("@DescrOfAbility", thisAbility.DescrOfAbility);
                int? abilIsActive = null;
                if (thisAbility.AbilIsActive == true)
                { abilIsActive = 1; }
                if (thisAbility.AbilIsActive == false)
                { abilIsActive = 0; }
                cmdAbil.Parameters.AddWithValue("@AbilIsActive", abilIsActive);
                cmdAbil.Parameters.AddWithValue("@ManaCost", thisAbility.ManaCost);
                cmdAbil.Parameters.AddWithValue("@ManaRegenCost", thisAbility.ManaRegenCost);
                cmdAbil.Parameters.AddWithValue("@HpCost", thisAbility.HpCost);
                cmdAbil.Parameters.AddWithValue("@DurationMax", thisAbility.DurationMax);
                cmdAbil.Parameters.AddWithValue("@DurationElapsed", thisAbility.DurationElapsed);
                cmdAbil.Parameters.AddWithValue("@NoOfEntitiesAffectedMax", thisAbility.NoOfEntitiesAffectedMax);
                cmdAbil.Parameters.AddWithValue("@TargetEntitiesAffected", thisAbility.TargetEntitiesAffected);
                cmdAbil.Parameters.AddWithValue("@HpEffect", thisAbility.HpEffect);
                cmdAbil.Parameters.AddWithValue("@TargetRange", thisAbility.TargetRange);
                cmdAbil.Parameters.AddWithValue("@SaveType", thisAbility.SaveType);
                cmdAbil.ExecuteNonQuery();
                con.Close();

            }
        }

        private void SQLSaveFantEntity(Fant_Entity selected)
        {   //?? is it better to do this as a Using or just make sure I close the connection?

            InsertObjInSQLInv(selected);

            InsertTargetsSQLTargets(selected);

            InsertAbilitiesSQLAbility(selected);

            con.Open();
            if (selected is Character)
            {
                Character charSelected = (Character)selected;
                SqlCommand cmd2 = new SqlCommand("Insert into Fant_Character (AC,HitOn20, Hp, InitMod, InitRoll, IsAlive, Lvl, MaxHp, MyTurn,Name, PartyName, " +
                      "CharType, Chr, Con, Dex, Exp, Inte, Str, Wis, MyTargetEnt, MyTargetParty," +
                      "MaxMana, CurrentMana, MaxManaRegen, ManaRegen, XPOnDefeat, DefeatMult) " +
                      "values  (@AC,@HitOn20, @Hp, @InitMod, @InitRoll, @IsAlive, @Lvl, @MaxHp,  @MyTurn,@Name, @PartyName," +
                      "@CharType, @Chr, @Con, @Dex, @Exp, @Inte, @Str, @Wis, @MyTargetEnt, @MyTargetParty," +
                      "@MaxMana, @CurrentMana, @MaxManaRegen, @ManaRegen, @XPOnDefeat, @DefeatMult )", con);     //taken out for now    @MyTargetEnt, @MyTargetParty, 
                cmd2.Parameters.AddWithValue("@AC", selected.AC);
                cmd2.Parameters.AddWithValue("@HitOn20", selected.HitOn20);
                cmd2.Parameters.AddWithValue("@Hp", selected.Hp);
                cmd2.Parameters.AddWithValue("@InitMod", selected.InitMod);
                cmd2.Parameters.AddWithValue("@InitRoll", selected.InitRoll);
                int? CharIsAlive = null;
                if (selected.IsAlive == true)
                { CharIsAlive = 1; }
                if (selected.IsAlive == false)
                { CharIsAlive = 0; }
                cmd2.Parameters.AddWithValue("@IsAlive", CharIsAlive);
                cmd2.Parameters.AddWithValue("@Lvl", selected.Lvl);
                cmd2.Parameters.AddWithValue("@MaxHp", selected.MaxHp);
                int? CharMyTurn = null;
                if (selected.MyTurn == true)
                { CharMyTurn = 1; }
                if (selected.MyTurn == false)
                { CharMyTurn = 0; }
                cmd2.Parameters.AddWithValue("@MyTurn", CharMyTurn);
                cmd2.Parameters.AddWithValue("@Name", selected.Name);
                cmd2.Parameters.AddWithValue("@PartyName", selected.PartyName);
                //new unique values for character
                cmd2.Parameters.AddWithValue("@CharType", charSelected.CharType);
                cmd2.Parameters.AddWithValue("@Chr", charSelected.Chr);
                cmd2.Parameters.AddWithValue("@Con", charSelected.Con);
                cmd2.Parameters.AddWithValue("@Dex", charSelected.Dex);
                cmd2.Parameters.AddWithValue("@Exp", charSelected.Exp);
                cmd2.Parameters.AddWithValue("@Inte", charSelected.Inte);
                cmd2.Parameters.AddWithValue("@Str", charSelected.Str);
                cmd2.Parameters.AddWithValue("@Wis", charSelected.Wis);
                cmd2.Parameters.AddWithValue("@MaxMana", charSelected.MaxMana);
                cmd2.Parameters.AddWithValue("@CurrentMana", charSelected.CurrentMana);
                cmd2.Parameters.AddWithValue("@MaxManaRegen", charSelected.MaxManaRegen);
                cmd2.Parameters.AddWithValue("@ManaRegen", charSelected.ManaRegen);
                cmd2.Parameters.AddWithValue("@XPOnDefeat", charSelected.XPOnDefeat);
                cmd2.Parameters.AddWithValue("@DefeatMult", charSelected.DefeatMult);
                if (selected.MyTargetEnt != null)
                {
                    cmd2.Parameters.AddWithValue("@MyTargetEnt", selected.MyTargetEnt);
                    cmd2.Parameters.AddWithValue("@MyTargetParty", selected.MyTargetParty);
                }
                else
                {
                    cmd2.Parameters.AddWithValue("@MyTargetEnt", "");
                    cmd2.Parameters.AddWithValue("@MyTargetParty", "");
                }
                cmd2.ExecuteNonQuery();
            }
            if (selected is Monster)
            {
                Monster monstSelected = (Monster)selected;
                SqlCommand cmd3 = new SqlCommand("Insert into Monster (AC, MonsterType, HitOn20, Hp, InitMod, InitRoll, IsAlive," +
                    " MaxHp, MyTargetEnt, MyTargetParty,MyTurn,Name, PartyName, Lvl, NoOfAtt, DamPerAtt1, DamPerAtt2, DamPerAtt3, HitDie," +
                    " MaxMana, CurrentMana, MaxManaRegen, ManaRegen, XPOnDefeat, DefeatMult)" +
                    " values (@AC, @MonsterType, @HitOn20, @Hp, @InitMod, @InitRoll, @IsAlive, @MaxHp, @MyTargetEnt,@MyTargetParty," +
                    "@MyTurn,@Name,@PartyName, @Lvl,@NoOfAtt, @DamPerAtt1, @DamPerAtt2, @DamPerAtt3, @HitDie," +
                    " @MaxMana, @CurrentMana, @MaxManaRegen, @ManaRegen, @XPOnDefeat, @DefeatMult)", con);
                cmd3.Parameters.AddWithValue("@AC", monstSelected.AC);
                cmd3.Parameters.AddWithValue("@MonsterType", monstSelected.MonsterType);
                cmd3.Parameters.AddWithValue("@HitOn20", monstSelected.HitOn20);
                cmd3.Parameters.AddWithValue("@Hp", monstSelected.Hp);
                cmd3.Parameters.AddWithValue("@InitMod", monstSelected.InitMod);
                cmd3.Parameters.AddWithValue("@InitRoll", monstSelected.InitRoll);
                cmd3.Parameters.AddWithValue("@IsAlive", monstSelected.IsAlive);
                cmd3.Parameters.AddWithValue("@MaxHp", monstSelected.MaxHp);
                if (monstSelected.MyTargetEnt != null)
                {
                    cmd3.Parameters.AddWithValue("@MyTargetEnt", monstSelected.MyTargetEnt);
                    cmd3.Parameters.AddWithValue("@MyTargetParty", monstSelected.MyTargetParty);
                }
                else
                {
                    cmd3.Parameters.AddWithValue("@MyTargetEnt", "");
                    cmd3.Parameters.AddWithValue("@MyTargetParty", "");
                }
                cmd3.Parameters.AddWithValue("@MyTurn", monstSelected.MyTurn);
                cmd3.Parameters.AddWithValue("@Name", monstSelected.Name);
                cmd3.Parameters.AddWithValue("@PartyName", monstSelected.PartyName);
                cmd3.Parameters.AddWithValue("@Lvl", monstSelected.Lvl);
                cmd3.Parameters.AddWithValue("@NoOfAtt", monstSelected.NoOfAtt);
                cmd3.Parameters.AddWithValue("@DamPerAtt1", monstSelected.DamPerAtt1);
                cmd3.Parameters.AddWithValue("@DamPerAtt2", monstSelected.DamPerAtt2);
                cmd3.Parameters.AddWithValue("@DamPerAtt3", monstSelected.DamPerAtt3);
                cmd3.Parameters.AddWithValue("@HitDie", monstSelected.HitDie);
                cmd3.Parameters.AddWithValue("@MaxMana", monstSelected.MaxMana);
                cmd3.Parameters.AddWithValue("@CurrentMana", monstSelected.CurrentMana);
                cmd3.Parameters.AddWithValue("@MaxManaRegen", monstSelected.MaxManaRegen);
                cmd3.Parameters.AddWithValue("@ManaRegen", monstSelected.ManaRegen);
                cmd3.Parameters.AddWithValue("@XPOnDefeat", monstSelected.XPOnDefeat);
                cmd3.Parameters.AddWithValue("@DefeatMult", monstSelected.DefeatMult);

                cmd3.ExecuteNonQuery();
            }
            con.Close();
            UpdateSQLList();
        }

        private void UpdateInSQL_Click(object sender, RoutedEventArgs e)
        {
            Fant_Entity selected = MainWindow.entitySelected;
            SQLUpdateFantEntity(selected);
        }

        private void Fant_Ents_inSQL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Fant_Ents_inSQL.SelectionChanged += Fant_Ents_inSQL_SelectionChanged;

        }

        private void SqlDataGridUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateSQLList();
        }
        #endregion SQL Load and Save
        private void UpdateAllListBoxes()
        {
            UpdatePartyListBox();
            UpdateMonstPartyListBox();
            UpdateEntPartiesListBox();
            UpdateEntPartyListBox();
            UpdateTargetFocusGroupListBox();
            UpdateTargetFocusCharListBox();
            UpdateMonstPartiesListBox();
            UpdatePartiesListBox();

        }

        private void ClearDialog_Click(object sender, RoutedEventArgs e)
        {
            File.Delete(comScriptPath);
            CombatDialog.Text = "Combat Log Cleared.";
        }

        private void RollDieDM_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CalcXP_Click(object sender, RoutedEventArgs e)
        {
            CalculateXP();
        }
        private void CalculateXP()
        {
            double outnumberedMod = 1;
            double calculatedXP = 0;
            double aveLvlDefPArty = 0;
            int cumulLvlsDef = 0;
            int cumLvlsAttack = 0;
            double aveLvlAttParty = 0;
            Party defeatedParty = (Party)TargetFocusGroupList.SelectedItem;
            Party attackingParty = (Party)EntGroupList.SelectedItem;
            foreach (Fant_Entity thisEntity in defeatedParty)
            {
                int lvlHPCalcs = thisEntity.Lvl;
                int hpXPCalcs = thisEntity.MaxHp;
                double defeatMult = thisEntity.DefeatMult;
                CalculateXP thisCalc = new CalculateXP(lvlHPCalcs, hpXPCalcs, defeatMult);
                calculatedXP += thisCalc.XPForDefeatCalc();
                cumulLvlsDef += thisEntity.Lvl;
            }
            calculatedXP = Math.Round(calculatedXP, 0);
            CombatScript($"{defeatedParty.Name} is worth {calculatedXP} experience points, unadjusted for difficulty");


            foreach (Fant_Entity attEntity in attackingParty)                     //at the moment this just takes into account 
            { cumLvlsAttack += attEntity.Lvl; }                                   // too many attackers, or attackers too high a level.
            aveLvlDefPArty = cumulLvlsDef / defeatedParty.Count;                  // the reverse is sort of taken care off as with inc. XP per level. 
            aveLvlAttParty = cumLvlsAttack / attackingParty.Count;
            if (aveLvlAttParty >= aveLvlDefPArty + 9)
            { outnumberedMod = 0; }
            if (aveLvlAttParty >= aveLvlDefPArty + 6)
            {
                if (attackingParty.Count / defeatedParty.Count > 6)
                { outnumberedMod = .0025; }
                else if (attackingParty.Count / defeatedParty.Count > 3)
                { outnumberedMod = .075; }
                else if (attackingParty.Count / defeatedParty.Count > 2)
                { outnumberedMod = .15; }
                else if (attackingParty.Count / defeatedParty.Count > 1.5)
                { outnumberedMod = .2125; }
                else
                { outnumberedMod = .25; }
            }
            else if (aveLvlAttParty >= aveLvlDefPArty + 3)
            {
                if (attackingParty.Count / defeatedParty.Count > 6)
                { outnumberedMod = .05; }
                else if (attackingParty.Count / defeatedParty.Count > 3)
                { outnumberedMod = .125; }
                else if (attackingParty.Count / defeatedParty.Count > 2)
                { outnumberedMod = .30; }
                else if (attackingParty.Count / defeatedParty.Count > 1.5)
                { outnumberedMod = .425; }
                else
                { outnumberedMod = .50; }
            }
            else
            {
                if (attackingParty.Count / defeatedParty.Count > 6)
                { outnumberedMod = .1; }
                else if (attackingParty.Count / defeatedParty.Count > 3)
                { outnumberedMod = .25; }
                else if (attackingParty.Count / defeatedParty.Count > 2)
                { outnumberedMod = .60; }
                else if (attackingParty.Count / defeatedParty.Count > 1.5)
                { outnumberedMod = .85; }

            }
            calculatedXP *= outnumberedMod;
            calculatedXP = Math.Round(calculatedXP, 0);           // yes I know this introduces rounding errors but it isn't important
            CombatScript($"For{attackingParty.Name}, {defeatedParty.Name} is worth {calculatedXP} experience points, after difficulty adjustments");
            calculatedXP /= attackingParty.Count;             //this is equivalent to calculatedXP/attacking party.Count
            calculatedXP = Math.Round(calculatedXP, 0);
            CombatScript($"which works out at {calculatedXP} per attacker.");
            var combatTxt = File.ReadAllText(comScriptPath);
            CombatDialog.Text = combatTxt;

            MessageBoxResult xPAddToEntities = MessageBox.Show("Would you like to add the final calculated XP \n" +
                " divided by number of attackers to attacking group", "Add Experience", MessageBoxButton.YesNo);
            switch (xPAddToEntities)
            {
                case MessageBoxResult.Yes:
                    foreach (Fant_Entity thisEntity in attackingParty)
                    {
                        if (thisEntity is Character)
                        {
                            Character thisChar = new Character();
                            thisChar = (Character)thisEntity;
                            thisChar.Exp += (int)calculatedXP;
                        }
                    }
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
        #region TCP Server stuff
        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            //remember this starts both the  TCP chat server and the TCP command server 
            if (!int.TryParse(txtBoxServerPort.Text.Trim(), out int portNumberOut))
            {
                MessageBox.Show("Invalid port number supplied, return.");
                txtBoxServerPort.Text = portNumb.ToString();
                return;
            }
            if (portNumberOut <= 49153 || portNumberOut > 65535)
            {
                MessageBox.Show("Port Number must be between 49153 and 65535.");
                txtBoxServerPort.Text = portNumb.ToString();
                return;
            }
            else portNumb = portNumberOut;
            portNumbCom = portNumb - 1;
            myServer.StartListeningForIncomingConnection(null, portNumb);


            portNumbCom = portNumb - 1;
            myServerCommands.StartListeningForIncomingConnectionCom(null, portNumbCom);

        }

        void HandleClientComConnected(object sender, ClientConnectedEventArgs ccea)
        {
            CombatScript($"{DateTime.Now} - New Tcp client (command) connected : {ccea.NewClient}  ");
            var combatTxt = File.ReadAllText(comScriptPath);
            CombatDialog.Text = combatTxt;
        }

        void HandleClientConnected(object sender, ClientConnectedEventArgs ccea)
        {
            CombatScript($"{DateTime.Now} - New Tcp client connected : {ccea.NewClient}  ");
            var combatTxt = File.ReadAllText(comScriptPath);
            CombatDialog.Text = combatTxt;
            // the combat script log may be getting a bit overused but it works for now.

        }
        // note that I will need another eventy handler =>HandleTextReceivedCom
        void HandleTextComReceived(object sender, TextReceivedEventArgs trea)
        {
            string receivedString = (trea.TextReceived);
            //need to put in some logic here so that multiple clients sending commands at once dont get messed up 
            finalStringRecCom += receivedString;
            //this is temp to get things set up 
            //think I will start/reset a timer at this point, after a certain number of milliseconds it will check if finalStringRecCom
            // is a stable set size and then convert to a character     
            if (!dispatcherTimerCom.IsEnabled)

            { StartTimerCheckForReceivedAll(); }
        }

        private void StartTimerCheckForReceivedAll()
        { 
       
            dispatcherTimerCom.Start();
        }
        private void OnTimerTickCom (object sender, EventArgs e)
        { if (finalStringRecCom != "")
            {
                string quickChkForStatic = finalStringRecCom;
                Thread.Sleep(20);
                //might need to change this sleep value to allow for reasonable network traffix
                if (finalStringRecCom == quickChkForStatic)
                {
                    dispatcherTimerCom.Stop();
                    //at the moment there are no commands coming from client, can do so later
                    xML = finalStringRecCom.Substring(5);
                    string commands = finalStringRecCom.Remove(5);
                    string inCom1 = commands.Substring(0, 3);
                    string inCom2 = commands.Substring(3, 2);
                    finalStringRecCom = "";

                    if (inCom1 == "01 ")
                    { UpdateCharacterFromPlayer(); }
                    if (inCom1 == "02 ")
                    {
                        MessageBox.Show($"{xML}");
                        //this works but I don't really know how to autoassign a character to a TCPclient as yet.
                        
                    }
                }
            }
        }
        private Character ReturnCharacter()
        {
            XmlSerializer xmlSerializer = new XmlSerializer(MainWindow.characterExample.GetType());
            Character transferChar = new Character();
            StringReader stringReader = new StringReader(xML);
            var what = xmlSerializer.Deserialize(stringReader);
            transferChar = (Character)what;

            return transferChar;
        }
        private void UpdateCharacterFromPlayer()
        {
           
           
           
            Character updateCharacter = ReturnCharacter();
            int myPartyIndex = MainWindow.CharParties.FindIndex(CharParty => CharParty.Name == updateCharacter.PartyName);
            int myCharIndex = MainWindow.CharParties[myPartyIndex].FindIndex(Character => Character.Name == updateCharacter.Name);
            MainWindow.CharParties[myPartyIndex][myCharIndex].Hp = updateCharacter.Hp;
            MainWindow.CharParties[myPartyIndex][myCharIndex].MaxHp = updateCharacter.MaxHp;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Str = updateCharacter.Str;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Inte = updateCharacter.Inte;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Wis = updateCharacter.Wis;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Dex = updateCharacter.Dex;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Con = updateCharacter.Con;
            MainWindow.CharParties[myPartyIndex][myCharIndex].MaxMana = updateCharacter.MaxMana;
            MainWindow.CharParties[myPartyIndex][myCharIndex].CurrentMana = updateCharacter.CurrentMana;
            MainWindow.CharParties[myPartyIndex][myCharIndex].MaxManaRegen = updateCharacter.MaxManaRegen;
            MainWindow.CharParties[myPartyIndex][myCharIndex].ManaRegen = updateCharacter.ManaRegen;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Lvl = updateCharacter.Lvl;
            MainWindow.CharParties[myPartyIndex][myCharIndex].Exp = updateCharacter.Exp;
            MainWindow.CharParties[myPartyIndex][myCharIndex].HitOn20 = updateCharacter.HitOn20;
            MainWindow.CharParties[myPartyIndex][myCharIndex].MyTurn = updateCharacter.MyTurn;
            MainWindow.CharParties[myPartyIndex][myCharIndex].InitMod = updateCharacter.InitMod;
            MainWindow.CharParties[myPartyIndex][myCharIndex].IsAlive = updateCharacter.IsAlive;
            MainWindow.CharParties[myPartyIndex][myCharIndex].MyTargetEnt = updateCharacter.MyTargetEnt;
            MainWindow.CharParties[myPartyIndex][myCharIndex].MyTargetParty = updateCharacter.MyTargetParty;

            MainWindow.CharParties[myPartyIndex][myCharIndex].Inventory.Clear();
            foreach (PhysObj physthing in updateCharacter.Inventory)
            { MainWindow.CharParties[myPartyIndex][myCharIndex].Inventory.Add(physthing); }

            MainWindow.CharParties[myPartyIndex][myCharIndex].Abilities.Clear();
            foreach (Ability thisAbility in updateCharacter.Abilities)
            { MainWindow.CharParties[myPartyIndex][myCharIndex].Abilities.Add(thisAbility); }

            MainWindow.CharParties[myPartyIndex][myCharIndex].MeleeTargets.Clear();
            foreach (Target thisTarget in updateCharacter.MeleeTargets)
            { MainWindow.CharParties[myPartyIndex][myCharIndex].MeleeTargets.Add(thisTarget); }

        }

        void HandleTextReceived(object sender, TextReceivedEventArgs trea)
        {
            CombatDialog.AppendText($"{DateTime.Now} - Received from {trea.ClientThatSentText} : {trea.TextReceived}");
            CombatDialog.AppendText(Environment.NewLine);
        }

        private void SendToAll_Click(object sender, RoutedEventArgs e)
        {
            //var combatTxt = File.ReadAllText(comScriptPath);
            //  CombatDialog.Text = combatTxt;
            myServer.SendToAll(CombatDialog.Text);
        }

        private void SendToSingleClient_Click(object sender, RoutedEventArgs e)
        {
            myServer.SendToTcpClient(CombatDialog.Text, Int32.Parse(ClientToSendTxt.Text));
        }

        //should probably put something in to stop Server on DmMainWindow exit.
        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            myServer.StopServer();
            myServerCommands.StopServer();
        }

        private void SendXMLEntity_Click(object sender, RoutedEventArgs e)
        {
            Fant_Entity selected = (Fant_Entity)EntCurrentPartyList.SelectedItem;
            string stringToSend = "";
            string _XML = ThisEntityToXMLString(selected);
            stringToSend = "01 01" + _XML;
            myServerCommands.SendToTcpClientCom(stringToSend, Int32.Parse(ClientToSendTxt.Text));   //was , 0
            //temporarily sending it to TCPClientCom[0] only 

        }

        private void UpdateXMLEntity_Click(object sender, RoutedEventArgs e)
        {
            Fant_Entity selected = (Fant_Entity)EntCurrentPartyList.SelectedItem;
            string stringToSend = "";
            string _XML = ThisEntityToXMLString(selected);
            stringToSend = "02 01" + _XML;
            myServerCommands.SendToTcpClientCom(stringToSend, Int32.Parse(ClientToSendTxt.Text));
        }
        private string ThisEntityToXMLString(Fant_Entity fant_Entity)
        {
            XmlSerializer _XMLformatter = new XmlSerializer(fant_Entity.GetType());
            StringWriter stringwriter = new StringWriter();
            string stringXML = "";
            _XMLformatter.Serialize(stringwriter, fant_Entity);
            stringXML = stringwriter.ToString();
            return stringXML;
        }

        private void ClientUpdatesCharacter_Click(object sender, RoutedEventArgs e)
        {
            UpdateCharacterFromPlayer();
        }

        #endregion TCP Server Stuff



        private void txtBoxServerPort_TextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void txtBoxServerPort_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ClientToSendTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void ClientToSendTxt_TextInput(object sender, TextCompositionEventArgs e)
        {
        }
      
    }
}
