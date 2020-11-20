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

namespace JBFantasyGame
{
    /// <summary>
    /// Interaction logic for PlayerMain.xaml
    /// </summary>
    public partial class PlayerMain : Window
    {
        JBAsynchTCPClient myTcpClient = new JBAsynchTCPClient();
        JBAsynchTCPClient myTcpClientCom = new JBAsynchTCPClient();
        string receivedString = "";
        string finalString = "";
        public PlayerMain()
        {
            InitializeComponent();
            myTcpClient.RaiseTextReceivedEvent += HandleTextReceived;
            myTcpClientCom.RaiseTextReceivedEvent += HandleTextReceivedCom;
            
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

        private void ReceiveXMLEntity_Click(object sender, RoutedEventArgs e)
        {
           // MessageBox.Show(finalString);
            XmlSerializer xmlSerializer = new XmlSerializer(MainWindow.characterExample.GetType());
            Character transferChar = new Character();
            StringReader stringReader = new StringReader(finalString);
            var what = xmlSerializer.Deserialize(stringReader);
            transferChar =(Character)what;
            ShowCharWin playerCharSheet = new ShowCharWin(transferChar);
            playerCharSheet.Show();
        }
    }
}
