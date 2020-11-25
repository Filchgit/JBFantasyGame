using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using JBAsyncTCPServer;
using System.IO;
using System.Windows;

namespace JBFantasyGame
{
    class JBAsynchTCPClient
    {
        IPAddress myServerIPAddress;
        int myServerPort;
        TcpClient myTcpClient;
        TcpClient myTcpClientCom;

        //adding the member variables to this class
        public EventHandler<TextReceivedEventArgs> RaiseTextReceivedEvent;

        public JBAsynchTCPClient()
        {  //declaring class constructor and setting default values
            myTcpClient = null;
            myTcpClientCom = null;

            myServerPort = -1;
            myServerIPAddress = null;

        }
        public IPAddress ServerIPAddress
        {
            get
            {
                return myServerIPAddress;
            }
        }
        public int ServerPort
        {
            get
            {
                return myServerPort;
            }
        }
        public bool SetServerIPAdress(string _IPAddressServer)
        {
            IPAddress ipaddr = null;
            if (!IPAddress.TryParse(_IPAddressServer, out ipaddr))
            {
                MessageBox.Show("Invalid server IP supplied.");
                return false;
            }

            myServerIPAddress = ipaddr;
            return true;

        }

        internal async Task SendToServer(string strInputUser)
        {
            if (string.IsNullOrEmpty(strInputUser))
            {
                MessageBox.Show("Empty string supplied to send.");
                return;
            }
            if (myTcpClient != null)
            {
                if (myTcpClient.Connected)
                {
                    StreamWriter clientStreamWriter = new StreamWriter(myTcpClient.GetStream());
                    clientStreamWriter.AutoFlush = true;
                    // clientStreamWriter.AutoFlush being true means every it will 
                    //flush its buffer to underlying stream after every call to StreamWriter.Write(char)

                    await clientStreamWriter.WriteAsync(strInputUser);
                   // Console.WriteLine("Data sent.");
                }
            }
        }

        internal async Task SendToServerCom(string strInputUser)
        {
            if (string.IsNullOrEmpty(strInputUser))
            {
                MessageBox.Show("Empty string supplied to send.");
                return;
            }
            if (myTcpClientCom != null)
            {
                if (myTcpClientCom.Connected)
                {
                    StreamWriter clientStreamWriter = new StreamWriter(myTcpClientCom.GetStream());
                    clientStreamWriter.AutoFlush = true;
                    // clientStreamWriter.AutoFlush being true means every it will 
                    //flush its buffer to underlying stream after every call to StreamWriter.Write(char)

                    await clientStreamWriter.WriteAsync(strInputUser);
                    // Console.WriteLine("Data sent.");
                }
            }
        }
        public void CloseAndDisconnect()
        {
            if (myTcpClient != null)
            {
                if (myTcpClient.Connected)
                {
                    myTcpClient.Close();
                }
            }
        }
        protected virtual void OnRaisedTextReceivedEvent(TextReceivedEventArgs trea)
        {
            EventHandler<TextReceivedEventArgs> handler = RaiseTextReceivedEvent;
            if (handler != null)
            {
                handler(this, trea);
            }

        }

        public bool SetPortNumber(string _ServerPort)
        {
            int portNumber = 0;

            if (!int.TryParse(_ServerPort.Trim(), out portNumber))
            {
                MessageBox.Show("Invalid port number supplied, return.");
                return false;
            }
            if (portNumber <= 0 || portNumber > 65535)
            {
                MessageBox.Show("Port Number must be between 0 and 65535.");
                return false;
            }

            myServerPort = portNumber;
            return true;
        }
        public async Task ConnectToServer()
        // ok here is a new thing to note; the Task keyword is used for async methods that don't have a return,
        // much like void is used for 'normal synchronous' methods. Might be good for you to remember that
        //asynch is not an abstraction over threading.
        {
            if (myTcpClient == null)
            {
                myTcpClient = new TcpClient();
            }

            try
            {
                await myTcpClient.ConnectAsync(myServerIPAddress, myServerPort);
                MessageBox.Show($"Connected to server IP/Port: {myServerIPAddress} / {myServerPort}");

                ReadDataAsync(myTcpClient);
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.ToString());
                throw;
            }
        }
        public async Task ConnectToServerCom()
        {
            if (myTcpClientCom == null)
            {
                myTcpClientCom = new TcpClient();
            }

            try
            {
                
                myServerPort -= 1;
                await myTcpClientCom.ConnectAsync(myServerIPAddress, myServerPort);
                MessageBox.Show($"Connected to Command server IP/Port: {myServerIPAddress} / {myServerPort}");
                // this will later send from a saved file that the game gets on loading 
                SendToServerCom("02   JBAlias");
                await ReadDataAsync(myTcpClientCom);
            }
            catch (Exception excp)
            {
                MessageBox.Show(excp.ToString());
                throw;
            }



        }





        private async Task ReadDataAsync(TcpClient myTcpClient)
        {
            try
            {
                StreamReader clientStreamReader = new StreamReader(myTcpClient.GetStream());
                char[] buff = new char[64];
                int readByteCount = 0;

                while (true)
                {
                    readByteCount = await clientStreamReader.ReadAsync(buff, 0, buff.Length);

                    if (readByteCount <= 0)
                    {
                        MessageBox.Show("Disconnected from server.");
                        myTcpClient.Close();
                        break;
                    }

                    // Console.WriteLine(string.Format("Received bytes: {0} - Message {1} ", readByteCount, new string(buff)));

                    OnRaisedTextReceivedEvent(
                     new TextReceivedEventArgs(myTcpClient.Client.RemoteEndPoint.ToString(),
                     new string(buff)));

                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show($"{excp.ToString()}");
                throw;
            }
        }
    }

}

