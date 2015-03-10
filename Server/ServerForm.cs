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

namespace Server
{
    public partial class ServerForm : Form
    {
        private static byte[] tcpBuffer = new byte[1024];
        private static byte[] udpBuffer = new byte[1024];
        private static Socket tcpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Socket udpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static ConcurrentQueue<string> textIn = new ConcurrentQueue<string>();
        private static ConcurrentQueue<string> textOut = new ConcurrentQueue<string>();
        private static List<Socket> tcpClients = new List<Socket>();
        private static List<Socket> udpClients = new List<Socket>();


        public ServerForm()
        {
            InitializeComponent();
        }

        private void bUDP_Click(object sender, EventArgs e)
        {

        }

        private void bTCP_Click(object sender, EventArgs e)
        {

        }

        private void SetupTCPServer()
        {
            textOut.Enqueue("Setting up TCP server...");
            tcpServerSocket.Bind(new IPEndPoint(IPAddress.Any, (int)tcpPort.Value));
            tcpServerSocket.Listen(5);
            tcpServerSocket.BeginAccept(new AsyncCallback(AcceptTCPCallBack), null);
        }

        private static void AcceptTCPCallBack(IAsyncResult AR)
        {
            Socket socket = tcpServerSocket.EndAccept(AR);
            tcpClients.Add(socket);
            socket.BeginReceive(tcpBuffer, 0, tcpBuffer.Length, SocketFlags.None, new AsyncCallback(RecieveTCPCallback), socket);
            tcpServerSocket.BeginAccept(new AsyncCallback(AcceptTCPCallBack), null);
        }

        private static void RecieveTCPCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int recieved = socket.EndReceive(AR);
            byte[] dataBuffer = new byte[recieved];
            Array.Copy(tcpBuffer, dataBuffer, recieved);
            string text = Encoding.ASCII.GetString(dataBuffer);
            textIn.Enqueue("TCP: " + text);



        }

        public static void SendText(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);

        }
        private void SetupUDPServer()
        {
            
        }
    }
}
