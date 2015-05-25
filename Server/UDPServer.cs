using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml;
using System.IO;

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
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, ui.TCPPort);
                udpSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref newClientEP, ReceiveCallback, udpSocket);

            }
            catch (SocketException se)
            {
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


        public void ConvertByteToXml(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                readStream(stream);
            }
        }
        public void readStream(MemoryStream stream)
        {
            using (XmlReader reader = XmlReader.Create(stream))
            {

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "RIst":
                                string RIstValue;
                                RIstValue = reader["X"];/*
                                if (RIstValue != null)
                                {
                                    measuredPosition.X = float.Parse(RIstValue);
                                }
                                RIstValue = reader["Y"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.Y = float.Parse(RIstValue);
                                }
                                RIstValue = reader["Z"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.Z = float.Parse(RIstValue);
                                }
                                RIstValue = reader["A"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.A = float.Parse(RIstValue);
                                }
                                RIstValue = reader["B"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.B = float.Parse(RIstValue);
                                }
                                RIstValue = reader["C"];
                                if (RIstValue != null)
                                {
                                    measuredPosition.Y = float.Parse(RIstValue);
                                }
                                break;
                            case "RSol":
                                string RSolValue;
                                RSolValue = reader["X"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.X = float.Parse(RSolValue);
                                }
                                RSolValue = reader["Y"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.Y = float.Parse(RSolValue);
                                }
                                RSolValue = reader["Z"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.Z = float.Parse(RSolValue);
                                }
                                RSolValue = reader["A"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.A = float.Parse(RSolValue);
                                }
                                RSolValue = reader["B"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.B = float.Parse(RSolValue);
                                }
                                RSolValue = reader["C"];
                                if (RSolValue != null)
                                {
                                    measuredSentPosition.Y = float.Parse(RSolValue);
                                }
                                break;
                            case "AIPos":
                                string AIPosValue;
                                AIPosValue = reader["A1"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A1 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A2"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A2 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A3"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A3 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A4"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A4 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A5"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A5 = float.Parse(AIPosValue);
                                }
                                AIPosValue = reader["A6"];
                                if (AIPosValue != null)
                                {
                                    measuredAngles.A6 = float.Parse(AIPosValue);
                                }
                                break;
                            case "ASPos":
                                string ASPosValue;
                                ASPosValue = reader["A1"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A1 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A2"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A2 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A3"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A3 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A4"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A4 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A5"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A5 = float.Parse(ASPosValue);
                                }
                                ASPosValue = reader["A6"];
                                if (ASPosValue != null)
                                {
                                    measuredSentAngles.A6 = float.Parse(ASPosValue);
                                }
                                break;
                            case "MACur":
                                string MACurValue;
                                MACurValue = reader["A1"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A1 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A2"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A2 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A3"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A3 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A4"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A4 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A5"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A5 = float.Parse(MACurValue);
                                }
                                MACurValue = reader["A6"];
                                if (MACurValue != null)
                                {
                                    measuredCurrent.A6 = float.Parse(MACurValue);
                                }
                                break;
                            case "IPOC":
                                IPoc = long.Parse(reader.Value.Trim());*/
                                break;
                        }
                    }
                }
            }
        }



        #endregion
    }
}
