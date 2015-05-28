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
using System.Net.NetworkInformation;
using System.Diagnostics;

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
        private float[] KukaPos;
        private float[] KukaPosSent;
        private long Ipoc;
        private bool hasUpdatedIpoc;
        


        #endregion

        #region Properties
        public bool IsActive { get { return serverIsActive; } }
        #endregion

        #region Constructor

        public UDPServer(ServerForm UI)
        {
            ui = UI;
            KukaPos = new float[6];
            KukaPosSent = new float[6];
            hasUpdatedIpoc = false;
            InitialiseUDPSocket();
            ui.UpdateText(": Availiable networks:", ui.InfoWindow);
            findIP();

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

        private void findIP()
        {
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                ui.UpdateText("Name: " + netInterface.Name, ui.InfoWindow);
                ui.UpdateText("Description: " + netInterface.Description, ui.InfoWindow);
                ui.UpdateText("Addresses: ", ui.InfoWindow);
                IPInterfaceProperties ipProps = netInterface.GetIPProperties();
                foreach (UnicastIPAddressInformation addr in ipProps.UnicastAddresses)
                {
                    ui.UpdateText(" " + addr.Address.ToString(), ui.InfoWindow);
                }
                ui.UpdateText("", ui.InfoWindow);
            }
        }

        public bool StartListening()
        {
            try
            {
                ui.UpdateText(": Starting UDP server", ui.InfoWindow);
                buffer = new byte[1024];
                endpoint = new IPEndPoint(IPAddress.Parse(ui.UDPip), ui.UDPPort);
                udpSocket.Bind(endpoint);
                EndPoint newClientEP = new IPEndPoint(IPAddress.Any, 0);
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
            Stopwatch receiveTimer = new Stopwatch();
            receiveTimer.Start();
            Socket receiveSocket = (Socket)ar.AsyncState;
            ui.UpdateText("Local: " + receiveSocket.LocalEndPoint.ToString(), ui.InfoWindow);
            ui.UpdateText("Remote: " + receiveSocket.RemoteEndPoint.ToString(), ui.InfoWindow);
            try
            {
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint tempRemoteEP = (EndPoint)sender;
                int dataLen = receiveSocket.EndReceiveFrom(ar, ref tempRemoteEP);
                ui.UpdateText(": UDP: active...", ui.InfoWindow);    
                byte[] data = new byte[dataLen];
                Array.Copy(buffer, data, dataLen);
                ui.ClearText(ui.MsgWindow);
                ui.UpdateText("UDP: " + Encoding.UTF8.GetString(data), ui.MsgWindow);
                sendByteData = ProcessData(data, tempRemoteEP);
                sendData(tempRemoteEP, sendByteData); 
                          
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
            string dataString = Encoding.UTF8.GetString(data);
            //ConvertByteToXml(data);
            // TODO: Update variables
            // TODO: Generate data to be sent back
            ui.UpdateText("Client: " + clientEP.ToString(), ui.MsgWindow);
            ui.UpdateText(dataString, ui.MsgWindow);
            
            string msg = "<Sen Type=\"ImFree\"><EStr></EStr><Tech T21=\"1.09\" T22=\"2.08\" T23=\"3.07\" T24=\"4.06\" T25=\"5.05\" T26=\"6.04\" T27=\"7.03\" T28=\"8.02\" T29=\"9.01\" T210=\"10.00\" /><RKorr X=\"0.000\" Y=\"0.000\" Z=\"0.000\" A=\"0.000\" B=\"0.000\" C=\"0.000\" /><DiO>125</DiO><IPOC>" + Ipoc.ToString() + "</IPOC></Sen>";
            return Encoding.UTF8.GetBytes(msg);
        }

        public String ConvertByteToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
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
                                RIstValue = reader["X"];
                                if (RIstValue != null)
                                {
                                    KukaPos[0] = float.Parse(RIstValue);
                                }
                                RIstValue = reader["Y"];
                                if (RIstValue != null)
                                {
                                    KukaPos[1] = float.Parse(RIstValue);
                                }
                                RIstValue = reader["Z"];
                                if (RIstValue != null)
                                {
                                    KukaPos[2] = float.Parse(RIstValue);
                                }
                                RIstValue = reader["A"];
                                if (RIstValue != null)
                                {
                                    KukaPos[3] = float.Parse(RIstValue);
                                }
                                RIstValue = reader["B"];
                                if (RIstValue != null)
                                {
                                    KukaPos[4] = float.Parse(RIstValue);
                                }
                                RIstValue = reader["C"];
                                if (RIstValue != null)
                                {
                                    KukaPos[5] = float.Parse(RIstValue);
                                }
                                break;
                            case "RSol":
                                string RSolValue;
                                RSolValue = reader["X"];
                                if (RSolValue != null)
                                {
                                    KukaPosSent[0] = float.Parse(RSolValue);
                                }
                                RSolValue = reader["Y"];
                                if (RSolValue != null)
                                {
                                    KukaPosSent[1] = float.Parse(RSolValue);
                                }
                                RSolValue = reader["Z"];
                                if (RSolValue != null)
                                {
                                    KukaPosSent[2] = float.Parse(RSolValue);
                                }
                                RSolValue = reader["A"];
                                if (RSolValue != null)
                                {
                                    KukaPosSent[3] = float.Parse(RSolValue);
                                }
                                RSolValue = reader["B"];
                                if (RSolValue != null)
                                {
                                    KukaPosSent[4] = float.Parse(RSolValue);
                                }
                                RSolValue = reader["C"];
                                if (RSolValue != null)
                                {
                                    KukaPosSent[5] = float.Parse(RSolValue);
                                }
                                break;/*
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
                                break;*/
                            case "IPOC":
                                Ipoc = long.Parse(reader.Value.Trim());
                                hasUpdatedIpoc = true;
                                break;
                        }
                    }
                }
            }
        }



        #endregion
    }
}
