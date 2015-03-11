using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        


        #endregion

        #region Properties

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
                ui.UpdateText("Exception: " + ex.Message, ui.TextIn);
            }
        }

        private void StopListening()
        {
            // TODO: impliment stopping function
        }

        public void StopServer()
        {
            udpSocket.Shutdown(SocketShutdown.Both);
            udpSocket.Shutdown(SocketShutdown.Receive);
            udpSocket.Close();
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        public void StartListening()
        {
            try
            {
                ui.UpdateText("Starting UDP server at " + DateTime.Now.ToString(), ui.TextIn);
                buffer = new byte[1024];
                endpoint = new IPEndPoint(IPAddress.Any, ui.UDPPort);
                udpSocket.Bind(endpoint);
                ui.UpdateText("Listening...", ui.TextIn);
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
                udpSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, ReceiveCallback, udpSocket);

            }
            catch (SocketException se)
            {
                ui.UpdateText("SocketException: " + se.Message, ui.TextIn);
            }
            catch (Exception ex)
            {
                ui.UpdateText("Exception: " + ex.Message, ui.TextIn);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            ui.UpdateText("UDP active...", ui.TextIn);
            Socket receiveSocket = (Socket)ar.AsyncState;
            EndPoint clientEP = new IPEndPoint(IPAddress.Any, 0);
            int dataLen = receiveSocket.EndReceiveFrom(ar, ref clientEP);
            byte[] data = new byte[dataLen];
            Array.Copy(buffer, data, dataLen);
            ui.UpdateText("UDP: " + Encoding.ASCII.GetString(data), ui.TextIn);
            sendByteData = ProcessData(data, clientEP);
            sendData(clientEP, sendByteData);
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
                ui.UpdateText("Exception: " + ex.Message, ui.TextIn);
            }
        }

        private void sendDataCallback(IAsyncResult ar)
        {
            try
            {
                udpSocket.EndSend(ar);
            }
            catch (Exception ex)
            {
                // TODO: exception handeling
                ui.UpdateText("Exception: " + ex.Message, ui.TextIn);
            }
        }

        private byte[] ProcessData(byte[] data, EndPoint clientEP)
        {
            // TODO: Read incomming data and convert to string
            // TODO: Update variables
            // TODO: Generate data to be sent back
            return new byte[]{0};
        }
        #endregion
    }
}
