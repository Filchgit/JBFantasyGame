using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class JBSocketServer
    {
        IPAddress myIP;
        int myPort;
        TcpListener myTCPListener;
        TcpListener myTCPListenerCom;

        List<TcpClient> myTcpClients;
        List<TcpClient> myTcpClientsCom;
        //I need to treat the command line TCP clients seperately 

        public EventHandler<ClientConnectedEventArgs> RaiseClientConnectedEvent;
        public EventHandler<TextReceivedEventArgs> RaiseTextReceivedEvent;

        public bool KeepRunning { get; set; }

        public JBSocketServer()
        {
            myTcpClients = new List<TcpClient>();
            myTcpClientsCom = new List<TcpClient>();
        }

        protected virtual void OnRaiseClientConnectedEvent(ClientConnectedEventArgs e)
        {
            EventHandler<ClientConnectedEventArgs> handler = RaiseClientConnectedEvent;

            if (handler != null)
            {
                handler(this, e);
            }
        }
       protected virtual void OnRaiseTextReceivedEvent(TextReceivedEventArgs trea)
        {
            EventHandler<TextReceivedEventArgs> handler = RaiseTextReceivedEvent;

            if (handler != null)
            {
                handler(this, trea);
            }
        }


        public async void StartListeningForIncomingConnection(IPAddress ipaddr = null, int port = 50000)
        // I need the async keyword in the method declare as I will be making an async call within it 
        // I am changing this to start two listeners at once
        {
            
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (port <= 0 || port >= 65535)
            {
                port = 50000;
              
            }
            myIP = ipaddr;
            myPort = port;

            System.Diagnostics.Debug.WriteLine(string.Format($"IP Address: {ipaddr}  - Port: {port} "));
            // since we are using.System Diagnostics we can skip the System.Diagnostics bit really

            myTCPListener = new TcpListener(myIP, myPort);

            try
            {
                myTCPListener.Start();

                KeepRunning = true;
                while (KeepRunning)
                {
                    var returnedByAccept = await myTCPListener.AcceptTcpClientAsync();


                    myTcpClients.Add(returnedByAccept);
                    //so if a new Tcp Client connects we add them to our Tcp Client List. . . . 
                    // I would like to do something with aliases here, tp refer correctly to each client
                    Debug.WriteLine(string.Format($"Client connected successfully, count" +
                        $" {myTcpClients.Count} - {returnedByAccept.Client.RemoteEndPoint}"
                        ));

                    TakeCareOfTCPClient(returnedByAccept);

                    ClientConnectedEventArgs eaClientConnected;
                    eaClientConnected = new ClientConnectedEventArgs(
                        returnedByAccept.Client.RemoteEndPoint.ToString()
                        );
                    OnRaiseClientConnectedEvent(eaClientConnected);

                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }

        }
        //trying to make a new async for commands port TCP
        public async void StartListeningForIncomingConnectionCom(IPAddress ipaddr = null, int portCom = 49999)
        // I need the async keyword in the method declare as I will be making an async call within it 
        // I am changing this to start two listeners at once
        {
            
            if (ipaddr == null)
            {
                ipaddr = IPAddress.Any;
            }
            if (portCom <= 49999 || portCom >= 65535)
            {
               
                portCom = 49999;
            }
            myIP = ipaddr;
            myPort = portCom;

            System.Diagnostics.Debug.WriteLine(string.Format($"IP Address: {ipaddr}  - Port: {portCom} "));
            // since we are using.System Diagnostics we can skip the System.Diagnostics bit really

            myTCPListenerCom = new TcpListener(myIP, myPort);

            try
            {
                myTCPListenerCom.Start();

                KeepRunning = true;
                while (KeepRunning)
                {
                    var returnedByAccept = await myTCPListenerCom.AcceptTcpClientAsync();


                    myTcpClientsCom.Add(returnedByAccept);
                    //so if a new Tcp Client connects we add them to our Tcp Client List. . . . 
                    // I would like to do something with aliases here, tp refer correctly to each client
                    Debug.WriteLine(string.Format($"Client connected successfully this one is a command Client, count" +
                        $" {myTcpClientsCom.Count} - {returnedByAccept.Client.RemoteEndPoint}"
                        ));

                    TakeCareOfTCPClientCom(returnedByAccept);

                    ClientConnectedEventArgs eaClientConnected;
                    eaClientConnected = new ClientConnectedEventArgs(
                        returnedByAccept.Client.RemoteEndPoint.ToString()
                        );
                    OnRaiseClientConnectedEvent(eaClientConnected);

                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }

        public void StopServer()
        {
            try
            { if (myTCPListener != null)
                {
                    myTCPListener.Stop();
                    // if I have a myTCPlistener instantiated then stop it. close the listener.
                }
                foreach (TcpClient thisTcpClient in myTcpClients)
                {
                    thisTcpClient.Close();
                }

            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
            myTcpClients.Clear();
            try
                {
                
                    if (myTCPListenerCom != null)
                    {
                        myTCPListenerCom.Stop();
                    }
                    foreach (TcpClient thisTcpClientCom in myTcpClientsCom)
                    {
                        thisTcpClientCom.Close();
                    }
                 }
          catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());

            }
            myTcpClientsCom.Clear();
        }
        private async void TakeCareOfTCPClient(TcpClient paramClient)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try 
            {
                stream = paramClient.GetStream();
                reader = new StreamReader(stream);
               
                char[] buff = new char[64];

                while(KeepRunning)
                {
                    Debug.WriteLine("**Ready to read.**");
                   int intReturned = await reader.ReadAsync(buff, 0, buff.Length);

                    Debug.WriteLine(string.Format($"Returned:" + intReturned));
                   
                    if (intReturned == 0)
                    {
                        RemoveTcpClient(paramClient);

                        Debug.WriteLine("Socket disconnected.");
                        //as a zero Intreturned means the stream has ended
                        break;
                    }
                    string receivedText = new string(buff);
                    
                    Debug.WriteLine (string.Format("Received: " + receivedText));
                    // need to clear the buff array after writing/using each time otherwise it will be garbled
                    OnRaiseTextReceivedEvent(new TextReceivedEventArgs(
                     paramClient.Client.RemoteEndPoint.ToString(),
                     receivedText
                        ));
                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch (Exception excp)
            {
                RemoveTcpClient(paramClient);

                Debug.WriteLine(excp.ToString());
            }
        }
        private async void TakeCareOfTCPClientCom(TcpClient paramClient)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            try
            {
                stream = paramClient.GetStream();
                reader = new StreamReader(stream);

                char[] buff = new char[64];

                while (KeepRunning)
                {
                    Debug.WriteLine("**Ready to read.**");
                    int intReturned = await reader.ReadAsync(buff, 0, buff.Length);

                    Debug.WriteLine(string.Format($"Returned:" + intReturned));

                    if (intReturned == 0)
                    {
 //coming back here need to fix all this to refer to TCPClientCom
                        
                        RemoveTcpClientCom(paramClient);

                        Debug.WriteLine("Socket disconnected.");
                        //as a zero Intreturned means the stream has ended
                        break;
                    }
                    string receivedText = new string(buff);

                    Debug.WriteLine(string.Format("Command Server Received: " + receivedText));
                    // need to clear the buff array after writing/using each time otherwise it will be garbled
                    OnRaiseTextReceivedEvent(new TextReceivedEventArgs(
                     paramClient.Client.RemoteEndPoint.ToString(),
                     receivedText
                        ));
                    Array.Clear(buff, 0, buff.Length);
                }
            }
            catch (Exception excp)
            {
                RemoveTcpClientCom(paramClient);

                Debug.WriteLine(excp.ToString());
            }
        }
        private void RemoveTcpClientCom(TcpClient paramClient)
        {
            if (myTcpClientsCom.Contains(paramClient))
            {
                myTcpClientsCom.Remove(paramClient);
                Debug.WriteLine($"client command removed, count {myTcpClientsCom.Count} ");
            }
        }
        private void RemoveTcpClient(TcpClient paramClient)
        {
          if(myTcpClients.Contains(paramClient))
            {
                myTcpClients.Remove(paramClient);
                Debug.WriteLine($"client removed, count {myTcpClients.Count} " );
            }
        }
        public async void SendToAll (string allMessage)
        {
            if (string.IsNullOrEmpty(allMessage))
            {
                return;
            }
            try 
            {
                byte[] buffMessage = Encoding.ASCII.GetBytes(allMessage);
                foreach (TcpClient thisTcpClient in myTcpClients)
                {
                    thisTcpClient.GetStream().WriteAsync(buffMessage, 0, buffMessage.Length);
                        //so this gets the networkstream associated with this TCP CLient and writes to it async
                }
            }
            catch (Exception excp)
            {
                Debug.WriteLine(excp.ToString());
            }
        }
    
    }
}
