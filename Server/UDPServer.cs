using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server
{
    class UDPServer
    {
        #region Fields
        private ServerForm ui;
        IPEndPoint endpoint;
        Socket udpSocket;
        private byte[] buffer;
        private byte[] sendByteData;
        private bool serverIsActive = false;
        


        #endregion

        #region Properties
        public bool IsActive { get { return serverIsActive; } }
        #endregion

        #region Constructor

        public UDPServer(ServerForm UI)
        {
            ui = UI;
            InitialiseUDPSocket();
        }
        #endregion

        #region Methods
        private void InitialiseUDPSocket()
        {
            try
            {
                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            catch (Exception ex)
            {
                // TODO: impliment error catching
                ui.UpdateText("Exception: " + ex.Message, ui.InfoWindow);
            }
        }

        private void StopListening()
        {
            // TODO: impliment stopping function
        }

        public void StopServer()
        {
            udpSocket.Shutdown(SocketShutdown.Both);
            udpSocket.Close();
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverIsActive = false;
            ui.UpdateText(": Sever is Dead.", ui.InfoWindow);
        }

        public bool StartListening()
        {
            try
            {
                ui.UpdateText(": Starting UDP server", ui.InfoWindow);
                buffer = new byte[1024];
                endpoint = new IPEndPoint(IPAddress.Any, ui.UDPPort);
                udpSocket.Bind(endpoint);
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                udpSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, ReceiveCallback, udpSocket);

            }
            catch (SocketException se)
            {
                ui.UpdateText(": UDP: Sever died.", ui.InfoWindow);
                ui.UpdateText(": UDP: SocketException: " + se.Message, ui.InfoWindow);
                return false;

            }
            catch (Exception ex)
            {
                ui.UpdateText(": UDP: Exception: " + ex.Message, ui.InfoWindow);
                return false;
            }
            ui.UpdateText(": UDP: Listening...", ui.InfoWindow);
            serverIsActive = true;
            return true;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket receiveSocket = (Socket)ar.AsyncState;
            try
            {
                
                EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
                int dataLen = receiveSocket.EndReceiveFrom(ar, ref clientEP);
                ui.UpdateText(": UDP: active...", ui.InfoWindow);    
                byte[] data = new byte[dataLen];
                Array.Copy(buffer, data, dataLen);
                ui.UpdateText("UDP: " + Encoding.ASCII.GetString(data), ui.MsgWindow);
                sendByteData = ProcessData(data, clientEP);
                sendData(clientEP, sendByteData); 
                          
            }
            catch (ObjectDisposedException)
            {
                ui.UpdateText(": UDP: Stopped Listening", ui.InfoWindow);
            }
            catch (Exception ex)
            {
                ui.UpdateText(": UDP: Exception: " + ex.Message, ui.InfoWindow);
            }
        }

        private void sendData(EndPoint clientEP, byte[] sendByteData)
        {
            try
            {
                udpSocket.BeginSendTo(sendByteData, 0, sendByteData.Length, SocketFlags.None, clientEP, sendDataCallback, udpSocket);
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                udpSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, ReceiveCallback, udpSocket);
            }
            catch (Exception ex)
            {
                // TODO: exception handeling
                ui.UpdateText(": UDP: Exception: " + ex.Message, ui.InfoWindow);
            }
        }

        private void sendDataCallback(IAsyncResult ar)
        {
            try
            {
                udpSocket.EndSend(ar);
                ui.UpdateText(": UDP: Sent Packet", ui.InfoWindow);
            }
            catch (Exception ex)
            {
                // TODO: exception handeling
                ui.UpdateText(": UDP: Exception: " + ex.Message, ui.InfoWindow);
            }
        }

        private byte[] ProcessData(byte[] data, EndPoint clientEP)
        {
            // TODO: Read incomming data and convert to string
            // TODO: Update variables
            // TODO: Generate data to be sent back
            string msg = "Got ya";
            return Encoding.ASCII.GetBytes(msg);
        }
        #endregion
    }
}
