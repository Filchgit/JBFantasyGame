using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace JBFantasyGame
{
    public class ClientConnectedEventArgs : EventArgs
    {
      public string NewClient { get; set; }
        public ClientConnectedEventArgs(string _newCLient)
        {
            NewClient = _newCLient;

        }
    
    }
    public class TextReceivedEventArgs : EventArgs
    {
        public string ClientThatSentText { get; set; }
        public string TextReceived { get; set; }
        public TextReceivedEventArgs(string _ClientThatSentText, string _textReceived)
        {
            ClientThatSentText = _ClientThatSentText;
            TextReceived = _textReceived;

        }
    }
}