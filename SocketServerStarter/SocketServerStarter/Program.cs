using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.Http.Headers;

namespace SocketServerStarter
{
    class Program
    {
        //tiny program to demonstrate sockets
        static void Main(string[] args)
        {
            Socket listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // creating a new socket where we will listen for incoming connections, using IP4, stream and TCP protocols
            IPAddress ipaddr = IPAddress.Any;
            // the IPAdress.Any sets the IP adress to the local host IP address
            IPEndPoint ipep = new IPEndPoint(ipaddr, 2300);
            // we are just assigning the socket number for the enpoint, socket numbers can be any number from  ____ to ____, though 1-____ are reserved
            listenerSocket.Bind(ipep);
            listenerSocket.Listen(5);
            // this tells the system how many clients can wait for a connection while the systems is busy handling other connection
            listenerSocket.Accept();
            // note that the Socket.Accept operation is a synchronous BLOCKING operation, (PURE EVIL -lol) nothing else happens until this operation is finished.


        }
    }
}
