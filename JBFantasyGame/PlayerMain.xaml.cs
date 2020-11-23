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
        private Character showCharacter = new Character();
     

        string receivedString = "";
        string finalString = "";
        string xML = "";
        private DispatcherTimer dispatcherTimerCommand = null;
        public PlayerMain()
        {
            InitializeComponent();
            myTcpClient.RaiseTextReceivedEvent += HandleTextReceived;
            myTcpClientCom.RaiseTextReceivedEvent += HandleTextReceivedCom;
          
            dispatcherTimerCommand = new DispatcherTimer();
            dispatcherTimerCommand.Interval = TimeSpan.FromSeconds(5.0);
            dispatcherTimerCommand.Tick += OnTimerTickCommand;
            dispatcherTimerCommand.Start();
    
        }
        private void OnTimerTickCommand(object sender, EventArgs e)                 // there was a tip to make sure that lengthy operations
        {   if (finalString != "")
            {
                string quickChkForStatic = finalString;
                Thread.Sleep(20);
                if (finalString == quickChkForStatic)
                {
                    xML = finalString.Substring(5);
                    string commands = finalString.Remove(5);
                    finalString = "";
                    string com1 = commands.Substring(0, 3);
                    string com2 = commands.Substring(3, 2);

                    if (com1 == "01 ")
                    { Character  aNewCharacter = ReturnCharacter();
                        if (MainWindow.CharParties.Count != 0)
                        {
                            //this stuff is for later in case I want to have them using multiple characters per player
                            foreach (CharParty chkCharParty in MainWindow.CharParties)
                            {
                                if (chkCharParty.Name == aNewCharacter.PartyName)
                                { chkCharParty.Add(aNewCharacter); }
                                else
                                {
                                    NewPlayerParty(aNewCharacter.PartyName);
                                    int myIndex = MainWindow.CharParties.FindIndex(CharParty => CharParty.Name == aNewCharacter.PartyName);
                                    MainWindow.CharParties[myIndex].Add(aNewCharacter);
                                }
                            }

                        }
                        NewPlayerParty(aNewCharacter.PartyName);
                       
                        MainWindow.CharParties[0].Add(aNewCharacter);
                        ShowCharWin showCharWin = new ShowCharWin(MainWindow.CharParties[0][0]);
                        showCharWin.Show();
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

        private void HandleTextReceivedCom (object sender, TextReceivedEventArgs trea)
        {
           // string receivedString="";
            //string finalString = "";
            receivedString = (trea.TextReceived);
       
            finalString += receivedString;
           
       

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

        private void ShowCharSheet_Click(object sender, RoutedEventArgs e)
        {
       
        }
    }
}
