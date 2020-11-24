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
using System.Xml.Serialization;
using System.IO;
using System.Windows.Threading;
using System.Threading;

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for PlayerMain.xaml
    /// </summary>
    public partial class PlayerMain : Window
    {
        JBAsynchTCPClient myTcpClient = new JBAsynchTCPClient();
        JBAsynchTCPClient myTcpClientCom = new JBAsynchTCPClient();
       
        public Character showCharacter = new Character();
        
   

        string receivedString = "";
        string finalString = "";
        string xML = "";
        private DispatcherTimer dispatcherTimerCommand = null;

        public PlayerMain()
        {
            InitializeComponent();
            myTcpClient.RaiseTextReceivedEvent += HandleTextReceived;
            myTcpClientCom.RaiseTextReceivedEvent += HandleTextReceivedCom;
           // showCharacter = MainWindow.characterExample;
           // ShowCharWin myCharWinShow = new ShowCharWin(showCharacter);
            dispatcherTimerCommand = new DispatcherTimer();
            dispatcherTimerCommand.Interval = TimeSpan.FromMilliseconds(200);
            dispatcherTimerCommand.Tick += OnTimerTickCommand;
           // dispatcherTimerCommand.Start();

           
          
        }
        private void OnTimerTickCommand(object sender, EventArgs e)                 // there was a tip to make sure that lengthy operations
        {   if (finalString != "")
            {
                string quickChkForStatic = finalString;
                Thread.Sleep(20);
                if (finalString == quickChkForStatic)
                {
                    dispatcherTimerCommand.Stop();
                 //actually might need to put a try /catch block in here in case my timing is off
                    xML = finalString.Substring(5);
                    string commands = finalString.Remove(5);
                    finalString = "";
                    string com1 = commands.Substring(0, 3);
                    string com2 = commands.Substring(3, 2);

                    if (com1 == "01 ")                                          //01 means add this as a new character
                    { Character  aNewCharacter = ReturnCharacter();
                        MainWindow.entitySelected = aNewCharacter;
                        if (MainWindow.CharParties.Count != 0)
                        {
                            bool thisPartyexist = false;
                            //this stuff is for later in case I want to have them using multiple characters per player
                            foreach (CharParty chkCharParty in MainWindow.CharParties)
                            {
                                if (chkCharParty.Name == aNewCharacter.PartyName)
                                {
                                    chkCharParty.Add(aNewCharacter);
                                    thisPartyexist = true;
                                }
                            }
                            if (thisPartyexist == false)
                            {
                                NewPlayerParty(aNewCharacter.PartyName);
                                int myIndex = MainWindow.CharParties.FindIndex(CharParty => CharParty.Name == aNewCharacter.PartyName);
                                MainWindow.CharParties[myIndex].Add(aNewCharacter);
                            }
                        }
                        else
                        {
                            NewPlayerParty(aNewCharacter.PartyName);
                            int myIndex = MainWindow.CharParties.FindIndex(CharParty => CharParty.Name == aNewCharacter.PartyName);
                            MainWindow.CharParties[myIndex].Add(aNewCharacter);
                        }

                       int myPartyIndex = MainWindow.CharParties.FindIndex(CharParty => CharParty.Name == aNewCharacter.PartyName);
                       int myCharIndex = MainWindow.CharParties[myPartyIndex].FindIndex(Character => Character.Name == aNewCharacter.Name);
                        ShowCharWin showCharWin = new ShowCharWin(MainWindow.CharParties[myPartyIndex][myCharIndex]);

                        showCharWin.Show();                      
                    }
                    if (com1 == "02 ")
                    { Character updateCharacter = ReturnCharacter();
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
                        foreach(PhysObj physthing in updateCharacter.Inventory)
                        { MainWindow.CharParties[myPartyIndex][myCharIndex].Inventory.Add(physthing);  }

                        MainWindow.CharParties[myPartyIndex][myCharIndex].Abilities.Clear();
                        foreach (Ability thisAbility in updateCharacter.Abilities)
                        { MainWindow.CharParties[myPartyIndex][myCharIndex].Abilities.Add(thisAbility); }

                        MainWindow.CharParties[myPartyIndex][myCharIndex].MeleeTargets.Clear();
                        foreach ( Target thisTarget in updateCharacter.MeleeTargets)
                        { MainWindow.CharParties[myPartyIndex][myCharIndex].MeleeTargets.Add(thisTarget); }

                        // so when I comeback I have to do this for all stats , dont forget to wipe and then add for abilities and targets
                    }


                }
            }
        }
        private void NewPlayerParty(string thispartyName )
        {
            CharParty charPlayerParty = new CharParty();
            Party entPlayerParty = new Party();
            MonsterParty monsterPlayerParty = new MonsterParty();
            charPlayerParty.Name = thispartyName;
            entPlayerParty.Name = thispartyName;
            monsterPlayerParty.Name = thispartyName;
            MainWindow.CharParties.Add(charPlayerParty);
            MainWindow.Parties.Add(entPlayerParty);
            MainWindow.MonsterParties.Add(monsterPlayerParty);

            
           
        }
       
        private void RollDiePlay_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void PlayRollDiceBtn_Click(object sender, RoutedEventArgs e)
        {
            String diecheck = RollDiePlay.Text;
            (int i1, int i2, int i3) = RollingDie.Diecheck(diecheck);
            if (i1 != 0)
            { 
                RollingDie thisRoll = new RollingDie(i1, i2,i3);
               // MessageBox.Show($"{thisRoll.Roll() } {RollDiePlay.Text }");   // we will make this talk out to a rolling chat box in a sec
                txtPlayerDialog.AppendText($" {thisRoll.Roll() } {RollDiePlay.Text } \n");
            }
        }

        private void RollDiePlay_TextInput(object sender, TextCompositionEventArgs e)
        {
            string var;
            var = RollDiePlay.Text;
            
        }
        private void HandleTextReceived(object sender, TextReceivedEventArgs trea)
        {
          //  string moreDialog =($" {DateTime.Now} - Received: {trea.TextReceived}");
            txtPlayerDialog.AppendText($"\n {trea.ClientThatSentText} : {trea.TextReceived}");
        }

        private void HandleTextReceivedCom(object sender, TextReceivedEventArgs trea)
        {
            // string receivedString="";
            //string finalString = "";
            receivedString = (trea.TextReceived);

            finalString += receivedString;

            if (!dispatcherTimerCommand.IsEnabled)
            { dispatcherTimerCommand.Start(); }
       

        }
        private void btnClearPlayerDialog_Click(object sender, RoutedEventArgs e)
        {
            txtPlayerDialog.Text = "Player Dialog Cleared";
        }

        private void btnConnectToServer_Click(object sender, RoutedEventArgs e)
        {
            string strIPAdress = txtDMsIpAddress.Text.Trim();
            
            int dMPortNumber;

            if (!myTcpClient.SetServerIPAdress(strIPAdress))
            {
                return;
            }
       
            
            if(!myTcpClient.SetPortNumber(txtDMsPort.Text.Trim()))
            { return; }

            myTcpClient.ConnectToServer();

            if (!myTcpClientCom.SetServerIPAdress(strIPAdress))
            {
                return;
            }

            if (!myTcpClientCom.SetPortNumber(txtDMsPort.Text.Trim()))
            { return; }

            myTcpClientCom.ConnectToServerCom();
        }
       
        public void SendToServerCom(string textToCommandServer)
        { myTcpClientCom.SendToServerCom(textToCommandServer); }

        private void btnPlayerSendTxt_Click(object sender, RoutedEventArgs e)
        {
            myTcpClient.SendToServer(txtOutgoingMessages.Text);
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

        private void UpdateShowCharacter()
        {
            
            XmlSerializer xmlSerializer = new XmlSerializer(MainWindow.characterExample.GetType());
            Character transferChar = new Character();
            StringReader stringReader = new StringReader(xML);
            var what = xmlSerializer.Deserialize(stringReader);
            showCharacter = (Character)what;
             
        }
        private void ReceiveXMLEntity_Click(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show(finalString);
            XmlSerializer xmlSerializer = new XmlSerializer(MainWindow.characterExample.GetType());
           
            StringReader stringReader = new StringReader(finalString);
            var what = xmlSerializer.Deserialize(stringReader);
            showCharacter =(Character)what;
            ShowCharWin playerCharSheet = new ShowCharWin(showCharacter);
            playerCharSheet.Show();
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

        private void SendCharacterToDM_Click(object sender, RoutedEventArgs e)
        {
            //at the moment just having one character per player so 
            string _XMLToSend = ThisEntityToXMLString(MainWindow.CharParties[0][0]);
            SendToServerCom(_XMLToSend);
        }
    }
}
