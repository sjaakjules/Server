using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TCPclient
{
    public partial class TCPForm : Form
    {
        private Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        delegate void UpdateTextCallback(string text);
        delegate void clearTextCallback();

        public TCPForm()
        {
            InitializeComponent();
        }

        private void bClientStart_Click(object sender, EventArgs e)
        {
            Thread connect = new Thread(new ThreadStart(LoopConnect));
            connect.Start();
        }

        private void UpdateTextIn(string text)
        {
            if (clientIn.InvokeRequired)
            {
                UpdateTextCallback d = new UpdateTextCallback(UpdateTextIn);
                Invoke(d, new object[] { text });
            }
            else
            {
                clientIn.AppendText(text);
            }

        }

        private void ClearText()
        {
            if (clientIn.InvokeRequired)
            {
                clearTextCallback d = new clearTextCallback(ClearText);
                Invoke(d);
            }
            else
            {
                clientIn.Clear();
            }
        }

        private void LoopConnect()
        {
            int attempts = 0;
            while (!clientSocket.Connected)
            {
                bool blockingState = clientSocket.Blocking;
                try
                {
                    attempts++;
                    clientSocket.Connect(IPAddress.Loopback, (int)clientPort.Value);
                    UpdateTextIn("Connected!");
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK 
                    if (e.NativeErrorCode.Equals(10035))
                        UpdateTextIn("Still Connected, but the Send would block");
                    else
                    {
                        ClearText();
                        UpdateTextIn("attempts: " + attempts);
                    }
                }
                finally
                {
                    clientSocket.Blocking = blockingState;
                }
            }            
        }


        private void SendText()
        {
            byte[] data = Encoding.ASCII.GetBytes(textOut.Text.Trim());
            clientSocket.Send(data);
        }

        private void TextOut_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendText();
            }
        }
    }
}
