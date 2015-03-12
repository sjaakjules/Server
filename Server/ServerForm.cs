using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace Server
{
    public partial class ServerForm : Form
    {
        // TCP Fields
        private byte[] tcpBuffer = new byte[1024];
        private Socket tcpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private List<Socket> tcpClients = new List<Socket>();

        // UDP Fields
        private UDPServer udpServer;

        // Delegates
        delegate void UpdateTextCallback(string text, TextBox textBox);
        delegate void ClearTextCallback(TextBox textBox);


        #region Properties
        public TextBox InfoWindow { get { return serverIn; } }
        public TextBox MsgWindow { get { return serverOut; } }
        public int UDPPort { get { return (int)udpPort.Value; } }

        #endregion

        public ServerForm()
        {
            InitializeComponent();
            udpServer = new UDPServer(this);
        }

        #region Methods

        #region Global methods

        private void udpPort_ValueChanged(object sender, EventArgs e)
        {
            if (udpServer.IsActive)
            {
                udpServer.StopServer();
                bUDP.Text = "Start UDP server";
            }
        }


        private void bUDP_Click(object sender, EventArgs e)
        {
            if (udpServer.IsActive)
            {
                udpServer.StopServer();
                bUDP.Text = "Start UDP server";
            }
            else if (udpServer.StartListening())
            {
                bUDP.Text = "Stop UDP server";                
            }
        }


        private void bTCP_Click(object sender, EventArgs e)
        {
            SetupTCPServer();
        }

        /// <summary>
        /// Thread safe method to append a TextBox
        /// </summary>
        /// <param name="text"> String of the text to be printed.</param>
        /// <param name="textBox">The TextBox object to which the string is appended</param>
        public void UpdateText(string text, TextBox textBox)
        {
            // Checks if we can add text now or will have to asyc it later
            if (textBox.InvokeRequired)
            {
                // Creates Asyc callback using the delegate defined
                UpdateTextCallback d = new UpdateTextCallback(UpdateText);
                textBox.Invoke(d,new object[]{text,textBox});
            }
            else
            {
                // No invoke required so will change text
                if (textBox.Name == serverIn.Name)
                {
                    text = DateTime.Now.ToLongTimeString() + text;
                }
                textBox.AppendText(text + System.Environment.NewLine);
            }
        }

        /// <summary>
        /// Thread safe method to clear a textBox
        /// </summary>
        /// <param name="textBox">The TextBox object which will be cleared</param>
        public void ClearText(TextBox textBox)
        {
            if (textBox.InvokeRequired)
            {
                ClearTextCallback d = new ClearTextCallback(ClearText);
                textBox.Invoke(d, new object[] { textBox });
            }
            else
            {
                textBox.Clear();
            }
        }
        
        public void SendText(string via, string text, Socket socket)
        {
            UpdateText(via + text, serverOut);
            byte[] sendData = Encoding.ASCII.GetBytes(text);
            socket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
        }

        private void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        #endregion

        #region TCP Methods
        /// <summary>
        /// Starts the TCP server with a new thread to listen to incomming connections
        /// </summary>
        private void SetupTCPServer()
        {
            UpdateText("Finding TCP clients...",serverOut);
            tcpServerSocket.Bind(new IPEndPoint(IPAddress.Any, (int)tcpPort.Value));
            tcpServerSocket.Listen(5);
            tcpServerSocket.BeginAccept(new AsyncCallback(AcceptTCPCallBack), null);
        }

        /// <summary>
        /// This method is called when a new client is connected which is Asyc called on a new thread
        /// </summary>
        /// <param name="AR"></param>
        private  void AcceptTCPCallBack(IAsyncResult AR)
        {
            Socket socket = tcpServerSocket.EndAccept(AR);
            UpdateText("TCP Client connected", serverIn);
            tcpClients.Add(socket);
            socket.BeginReceive(tcpBuffer, 0, tcpBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveTCPCallback), socket);
            tcpServerSocket.BeginAccept(new AsyncCallback(AcceptTCPCallBack), null);
        }

        /// <summary>
        /// This method is called when information is received which is Asyc called on a new thread
        /// </summary>
        /// <param name="AR"></param>
        private  void RecieveTCPCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int recieved = socket.EndReceive(AR);
            byte[] dataBuffer = new byte[recieved];
            Array.Copy(tcpBuffer, dataBuffer, recieved);
            string text = Encoding.ASCII.GetString(dataBuffer);
            UpdateText("TCP: " + text,serverIn);
            SendText("TCP: ","Msg revieved.",socket);
            socket.BeginReceive(tcpBuffer, 0, tcpBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveTCPCallback), socket);
        }

        #endregion



        #endregion
    }
}
