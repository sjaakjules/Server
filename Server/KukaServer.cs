using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
        // State object for reading client data asynchronously
    public class StateObject
    {
        // Client  socket.
        public Socket Socket = null;
        // Client EP.
        public EndPoint clientEP = null;
        // Client IPEP.
        public IPEndPoint clientIpEP = null;
        // Size of receive buffer.
        public int PacketInSize;
        // Receive buffer.
        public byte[] PacketIn;
        // Received data string.
        public string MessageIn;
        // Size of sending buffer.
        public int PacketOutSize;
        // Sending buffer.
        public byte[] PacketOut;
        // Sending XML document
        public XmlDocument XMLout;
        // Sending data string.
        public string MessageOut;
        // Received IPOC.
        public long IPOC = 0;
        // Received cartesian coordinates.
        public double[] cartPos = new double[6];
        // trigger that the message is loaded into the state holder
        public bool hasLoadedMessageOut;
    }

    class KukaServer
    {
        // Thread signals to pause until data has been received
        public ManualResetEvent haveReceived = new ManualResetEvent(false);


        public int _BufferSize = 1024;
        public byte[] _buffer;
        Socket _UdpSocket;
        IPEndPoint _localEP;
        int _Port = 6008;
        XmlDocument _SendXML;
        long _IPOC, _LIPOC;
        bool _isKukaServer = false;

        // Thread safe lists for updating and storing of robot information.
        public ConcurrentStack<StateObject> DataHistory;

        public Trajectory CurrentTrajectory;

        public RobotData _Robot;





        #region Constructor:
        /// <summary>
        /// Creates a UDP server with XML read and write via a port with threadSafe shared robot information
        /// </summary>
        /// <param name="port"></param> The port which communication occurs on
        /// <param name="robot"></param> The robot information to be updated and read from
        public KukaServer(int port, RobotData robot, bool isKuka)
        {
            _Robot = robot;
            _Port = port;
            _isKukaServer = isKuka;
            _IPOC = 0;
            _LIPOC = 0;


            // initialise external objects.
            DataHistory = new ConcurrentStack<StateObject>();


            SetupXML();


            // Create Socket
            string catchStatement = "while trying to create new socket:";
            try
            {
                _UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
            }
            // Binds the socket to local IP.
            bindSocket();
        }

      

        void setupDictionaries(ConcurrentDictionary<string, double> dic)
        {
            dic.TryAdd("X", 0);
            dic.TryAdd("Y", 0);
            dic.TryAdd("Z", 0);
            dic.TryAdd("A", 0);
            dic.TryAdd("B", 0);
            dic.TryAdd("C", 0);
        }

        #endregion


        #region General Server methods
        public void Shutdown()
        {

        }

        public void bindSocket()
        {
            // Finds local EP to bind socket
            _localEP = getAvailableIpEP();

            // Bind the local EP
            string catchStatement = " while trying to bind local EP:";
            try
            {
                _UdpSocket.Bind((EndPoint)_localEP);
                Console.WriteLine("Local IP bound: " + _UdpSocket.LocalEndPoint.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
            }
        }

        private IPEndPoint getAvailableIpEP()
        {
            string catchStatement = "while trying to get local IP endpoint:";
            try
            {
                // Finds the DNS name of the computer and prints to screen.
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
                Console.WriteLine(Dns.GetHostName().ToString());

                // Using this DNS displayes a list of available IP
                Console.WriteLine("Available IP: ");
                foreach (var IP in ipHostInfo.AddressList)
                {
                    Console.WriteLine(IP.ToString());
                }

                // Selects the first IP in the list and writes it to screen
                int addressSelction = Convert.ToInt32(Console.ReadKey(true).KeyChar)-48;
                Console.WriteLine(addressSelction);
                if (_isKukaServer)
                {
                    return new IPEndPoint(ipHostInfo.AddressList[addressSelction], _Port);
                }
                else
                {
                    return new IPEndPoint(ipHostInfo.AddressList[addressSelction], _Port);
                }
                

            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
                return new IPEndPoint(IPAddress.Any, 0);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
                return new IPEndPoint(IPAddress.Any, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
                return new IPEndPoint(IPAddress.Any, 0);
            }
        }

        public void oneOffSend(string msg)
        {

        }

        public void ConstantReceive()
        {
            while (true)
            {
                // resets the event to nonsignaled state.
                haveReceived.Reset();


                // Resets buffer and other variables loosing any unsaved data
                _buffer = new byte[_BufferSize];

                // Creates new State object which will have information for this dataGram communication.
                StateObject newState = new StateObject();
                newState.Socket = _UdpSocket;

                string catchStatement = "while trying to begin receiving data:";
                try
                {
                    newState.clientEP = (EndPoint)new IPEndPoint(IPAddress.Any, _Port); ;
                    _UdpSocket.BeginReceiveFrom(_buffer, 0, _BufferSize, SocketFlags.None, ref newState.clientEP, new AsyncCallback(FinishReceiveFrom), newState);
                    //      Console.WriteLine("Listening for data on port: {0}", _Port);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException {0}", catchStatement);
                    Console.WriteLine(se.Message);
                }
                catch (ObjectDisposedException ob)
                {
                    Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                    Console.WriteLine(ob.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Generic error {0}", catchStatement);
                    Console.WriteLine(e.Message);
                }
                // pause This thread until a packet has been returned.

                haveReceived.WaitOne();
            }

        }

        public void FinishReceiveFrom(IAsyncResult ar)
        {
            string catchStatement = "while receive data is active, reading data:";
            try
            {

                // get the object passed into the asyc function
                StateObject connectedState = (StateObject)ar.AsyncState;

                // end the receive and storing size of data in state
                connectedState.PacketInSize = connectedState.Socket.EndReceiveFrom(ar, ref connectedState.clientEP);
                // Initialize the state buffer with the received data size and copy buffer to state
                connectedState.PacketIn = new byte[connectedState.PacketInSize];
                Array.Copy(_buffer, connectedState.PacketIn, connectedState.PacketInSize);
                // Retrieve EP information and store in state
                connectedState.clientIpEP = (IPEndPoint)connectedState.clientEP;

                // Reset the global buffer to null ready to be initialised in next receive loop once packet as been sent.
                _buffer = null;

                // Process byte information on state object
                processData(connectedState);
                // Send return message to same connection that the data was received.
                SendData(connectedState);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
            }
        }

        private void FinishSendTo(IAsyncResult ar)
        {
            string catchStatement = "while trying to finsh sendTo:";
            try
            {
                //Console.WriteLine("Take that!");
                StateObject state = (StateObject)ar.AsyncState;
                int bytesSent = state.Socket.EndSendTo(ar);
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
            }
        }

        #endregion

        #region Mix kuka/processing methods

        private void SendData(StateObject state)
        {
            string catchStatement = "while trying to send the data:";
            try
            {
                if (state.hasLoadedMessageOut)
                {
                    state.Socket.BeginSendTo(state.PacketOut, 0, state.PacketOut.Length, SocketFlags.None, state.clientEP, new AsyncCallback(FinishSendTo), state);
                }
                else
                {
                    Console.WriteLine("Couldn't write message");
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
            }
            // Save state if it is a kuka server
            if (_isKukaServer)
            {
                DataHistory.Push(state);
            }
            haveReceived.Set();
        }


        private void processData(StateObject State)
        {
            string catchStatement = "while trying to process Data:";
            try
            {
                // Encode msg from state object
                State.MessageIn = Encoding.UTF8.GetString(State.PacketIn, 0, State.PacketInSize);
                if (_isKukaServer)
                {
                    KukaProcessData(State);
                }
                else
                {
                    externalProcessData(State);
                }

            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException {0}", catchStatement);
                Console.WriteLine(se.Message);
            }
            catch (ObjectDisposedException ob)
            {
                Console.WriteLine("ObjectDisposedException {0}", catchStatement);
                Console.WriteLine(ob.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Generic error {0}", catchStatement);
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.InnerException);
            }
        }
        #endregion

        #region Kuka specific methods

        private void SetupXML()
        {
            if (_isKukaServer)
            {
                _SendXML = new XmlDocument();
                XmlNode rootNode = _SendXML.CreateElement("Sen");
                XmlAttribute attribute = _SendXML.CreateAttribute("Type");
                attribute.Value = "ImFree";
                rootNode.Attributes.Append(attribute);
                _SendXML.AppendChild(rootNode);

                XmlNode errorNode = _SendXML.CreateElement("EStr");
                errorNode.InnerText = "";
                rootNode.AppendChild(errorNode);

                XmlNode techNode = _SendXML.CreateElement("Tech");
                attribute = _SendXML.CreateAttribute("T21");
                attribute.Value = "1.09";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T22");
                attribute.Value = "2.08";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T23");
                attribute.Value = "3.07";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T24");
                attribute.Value = "4.06";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T25");
                attribute.Value = "5.05";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T26");
                attribute.Value = "6.04";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T27");
                attribute.Value = "7.03";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T28");
                attribute.Value = "8.02";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T29");
                attribute.Value = "9.01";
                techNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("T210");
                attribute.Value = "10.00";
                techNode.Attributes.Append(attribute);
                rootNode.AppendChild(techNode);

                XmlNode comPosNode = _SendXML.CreateElement("RKorr");
                attribute = _SendXML.CreateAttribute("X");
                attribute.Value = "0.0000";
                comPosNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("Y");
                attribute.Value = "0.0000";
                comPosNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("Z");
                attribute.Value = "0.0000";
                comPosNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("A");
                attribute.Value = "0.0000";
                comPosNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("B");
                attribute.Value = "0.0000";
                comPosNode.Attributes.Append(attribute);
                attribute = _SendXML.CreateAttribute("C");
                attribute.Value = "0.0000";
                comPosNode.Attributes.Append(attribute);
                rootNode.AppendChild(comPosNode);

                XmlNode DiONode = _SendXML.CreateElement("DiO");
                DiONode.InnerText = "125";
                rootNode.AppendChild(DiONode);

                XmlNode IpocNode = _SendXML.CreateElement("IPOC");
                IpocNode.InnerText = "0";
                rootNode.AppendChild(IpocNode);
            }
            else
            {
                externalSetupXML();
            }

        }

        void KukaProcessData(StateObject State)
        {
            // create xml document from state message in.
            XmlDocument xmlIn = new XmlDocument();
            xmlIn.LoadXml(State.MessageIn);
            //Console.WriteLine(State.MessageIn);
            XmlNode IpocNode = xmlIn.SelectSingleNode("//Rob/IPOC");
            _LIPOC = _IPOC;
            if (long.TryParse(IpocNode.InnerText, out _IPOC))
            {
                //Console.WriteLine("IPOC: {0}", _IPOC);
                // Was succsessful, check if order is correct
                if (_LIPOC > _IPOC)
                {
                    Console.WriteLine("Error, packet order incorrect: New IPOC: {0} Old IPOC: {1}", _IPOC, _LIPOC);
                }
                State.IPOC = _IPOC;
            }
            else
            {
                Console.WriteLine("Error not reading IPOC: ");
            }
            _Robot.updateComandPosition();
            KukaWriteOut(State);

            // Update current position from xml document.
            XmlNode currentPosition = xmlIn.SelectSingleNode("//Rob/RIst");
            _Robot.updateRobotInfo(double.Parse(currentPosition.Attributes["X"].Value), double.Parse(currentPosition.Attributes["Y"].Value),
                            double.Parse(currentPosition.Attributes["Z"].Value), double.Parse(currentPosition.Attributes["A"].Value),
                            double.Parse(currentPosition.Attributes["B"].Value), double.Parse(currentPosition.Attributes["C"].Value));

        }


        public void KukaWriteOut(StateObject state)
        {
            XmlNode IpocNode = _SendXML.SelectSingleNode("//Sen/IPOC");
            IpocNode.InnerText = state.IPOC.ToString();

            XmlNode comPosNode = _SendXML.SelectSingleNode("//Sen/RKorr");
            comPosNode.Attributes["X"].Value = String.Format("{0:0.0000}", _Robot.CommandedPosition["X"]);
            comPosNode.Attributes["Y"].Value = String.Format("{0:0.0000}", _Robot.CommandedPosition["Y"]);
            comPosNode.Attributes["Z"].Value = String.Format("{0:0.0000}", _Robot.CommandedPosition["Z"]);
            comPosNode.Attributes["A"].Value = String.Format("{0:0.0000}", _Robot.CommandedPosition["A"]);
            comPosNode.Attributes["B"].Value = String.Format("{0:0.0000}", _Robot.CommandedPosition["B"]);
            comPosNode.Attributes["C"].Value = String.Format("{0:0.0000}", _Robot.CommandedPosition["C"]);

           // Console.WriteLine("ComandPosition: {0}:{1}:{2}", _Robot.CommandedPosition["X"], _Robot.CommandedPosition["Y"], _Robot.CommandedPosition["Z"]);
            flushCommandPosition();

            state.XMLout = (XmlDocument)_SendXML.Clone();

            state.PacketOut = Encoding.UTF8.GetBytes(Beautify(state.XMLout));
            state.hasLoadedMessageOut = true;

        }

        public string Beautify(XmlDocument doc)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = true,
                Indent = true,
                IndentChars = "",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                doc.Save(writer);
            }
            return sb.ToString();
        }

        public void flushCommandPosition()
        {

            _Robot.CommandedPosition["X"] = _Robot.CommandedPosition["X"] * 0.8;
            _Robot.CommandedPosition["Y"] = _Robot.CommandedPosition["Y"] * 0.8;
            _Robot.CommandedPosition["Z"] = _Robot.CommandedPosition["Z"] * 0.8;
            _Robot.CommandedPosition["A"] = _Robot.CommandedPosition["A"] * 0.8;
            _Robot.CommandedPosition["B"] = _Robot.CommandedPosition["B"] * 0.8;
            _Robot.CommandedPosition["C"] = _Robot.CommandedPosition["C"] * 0.8;
            
        }

        #endregion

        #region External controller specific methods

        void externalProcessData(StateObject State)
        {
            // create xml document from state message in.
            XmlDocument xmlIn = new XmlDocument();
            xmlIn.LoadXml(State.MessageIn);
            Console.WriteLine(State.MessageIn);
            // Update desired position from xml document.
            XmlNode desiredVelocity = xmlIn.SelectSingleNode("//Robot/Velocity");
            //updateVelocity(double.Parse(desiredVelocity.Attributes["X"].Value), double.Parse(desiredVelocity.Attributes["Y"].Value), double.Parse(desiredVelocity.Attributes["Z"].Value));
            

            // Update desired position from xml document.
            XmlNode desiredPosition = xmlIn.SelectSingleNode("//Robot/Position");
            updateDesiredPosition(double.Parse(desiredPosition.Attributes["X"].Value), double.Parse(desiredPosition.Attributes["Y"].Value), double.Parse(desiredPosition.Attributes["Z"].Value));
            //Console.WriteLine(State.MessageIn);
            externalWriteOut(State);




        }

        void updateVelocity(double x, double y, double z)
        {
            Console.WriteLine("{0}:{1}:{2}", x, y, z);
            _Robot.CommandedPosition["X"] = x;
            _Robot.CommandedPosition["Y"] = y;
            _Robot.CommandedPosition["Z"] = z;

        }

        void updateDesiredPosition(double x, double y, double z)
        {

            Console.WriteLine("New Position X: {0} Y: {1} Z: {2}", x, y, z);
            
            double[] desiredPosition = new double[] { x, y, z };
            _Robot.newPosition(desiredPosition);
            _Robot.goTo.startMovement();
        }

        void externalWriteOut(StateObject state)
        {
            XmlNode currentPosition = _SendXML.SelectSingleNode("//Robot/Position");
            currentPosition.Attributes["X"].Value = String.Format("{0:0.0000}", _Robot.ReadPosition["X"]);
            currentPosition.Attributes["Y"].Value = String.Format("{0:0.0000}", _Robot.ReadPosition["Y"]);
            currentPosition.Attributes["Z"].Value = String.Format("{0:0.0000}", _Robot.ReadPosition["Z"]);
            currentPosition.Attributes["A"].Value = String.Format("{0:0.0000}", _Robot.ReadPosition["A"]);
            currentPosition.Attributes["B"].Value = String.Format("{0:0.0000}", _Robot.ReadPosition["B"]);
            currentPosition.Attributes["C"].Value = String.Format("{0:0.0000}", _Robot.ReadPosition["C"]);

            XmlNode currentVelocity = _SendXML.SelectSingleNode("//Robot/Velocity");
            currentVelocity.Attributes["X"].Value = String.Format("{0:0.0000}", _Robot.Velocity["X"]);
            currentVelocity.Attributes["Y"].Value = String.Format("{0:0.0000}", _Robot.Velocity["Y"]);
            currentVelocity.Attributes["Z"].Value = String.Format("{0:0.0000}", _Robot.Velocity["Z"]);
            currentVelocity.Attributes["A"].Value = String.Format("{0:0.0000}", _Robot.Velocity["A"]);
            currentVelocity.Attributes["B"].Value = String.Format("{0:0.0000}", _Robot.Velocity["B"]);
            currentVelocity.Attributes["C"].Value = String.Format("{0:0.0000}", _Robot.Velocity["C"]);

            XmlNode currentAcceleration = _SendXML.SelectSingleNode("//Robot/Acceleration");
            currentAcceleration.Attributes["X"].Value = String.Format("{0:0.0000}", _Robot.acceleration["X"]);
            currentAcceleration.Attributes["Y"].Value = String.Format("{0:0.0000}", _Robot.acceleration["Y"]);
            currentAcceleration.Attributes["Z"].Value = String.Format("{0:0.0000}", _Robot.acceleration["Z"]);
            currentAcceleration.Attributes["A"].Value = String.Format("{0:0.0000}", _Robot.acceleration["A"]);
            currentAcceleration.Attributes["B"].Value = String.Format("{0:0.0000}", _Robot.acceleration["B"]);
            currentAcceleration.Attributes["C"].Value = String.Format("{0:0.0000}", _Robot.acceleration["C"]);

            state.XMLout = (XmlDocument)_SendXML.Clone();

            state.PacketOut = Encoding.UTF8.GetBytes(Beautify(state.XMLout));

            //Console.WriteLine(Encoding.UTF8.GetString(state.PacketOut));
            state.hasLoadedMessageOut = true;
        }

        public void UpdateTrajectory(double X, double Y, double Z)
        {
            //CurrentTrajectory = new Trajectory(new double[] { X, Y, Z });
        }


        private void externalSetupXML()
        {
            _SendXML = new XmlDocument();
            XmlNode rootNode = _SendXML.CreateElement("Robot");
            _SendXML.AppendChild(rootNode);

            XmlNode currentPosition = _SendXML.CreateElement("Position");
            XmlAttribute attribute = _SendXML.CreateAttribute("X");
            attribute.Value = "0.0000";
            currentPosition.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("Y");
            attribute.Value = "0.0000";
            currentPosition.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("Z");
            attribute.Value = "0.0000";
            currentPosition.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("A");
            attribute.Value = "0.0000";
            currentPosition.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("B");
            attribute.Value = "0.0000";
            currentPosition.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("C");
            attribute.Value = "0.0000";
            currentPosition.Attributes.Append(attribute);
            rootNode.AppendChild(currentPosition);

            XmlNode currentVelocity = _SendXML.CreateElement("Velocity");
            attribute = _SendXML.CreateAttribute("X");
            attribute.Value = "0.0000";
            currentVelocity.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("Y");
            attribute.Value = "0.0000";
            currentVelocity.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("Z");
            attribute.Value = "0.0000";
            currentVelocity.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("A");
            attribute.Value = "0.0000";
            currentVelocity.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("B");
            attribute.Value = "0.0000";
            currentVelocity.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("C");
            attribute.Value = "0.0000";
            currentVelocity.Attributes.Append(attribute);
            rootNode.AppendChild(currentVelocity);

            XmlNode currentAcceleration = _SendXML.CreateElement("Acceleration");
            attribute = _SendXML.CreateAttribute("X");
            attribute.Value = "0.0000";
            currentAcceleration.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("Y");
            attribute.Value = "0.0000";
            currentAcceleration.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("Z");
            attribute.Value = "0.0000";
            currentAcceleration.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("A");
            attribute.Value = "0.0000";
            currentAcceleration.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("B");
            attribute.Value = "0.0000";
            currentAcceleration.Attributes.Append(attribute);
            attribute = _SendXML.CreateAttribute("C");
            attribute.Value = "0.0000";
            currentAcceleration.Attributes.Append(attribute);
            rootNode.AppendChild(currentAcceleration);

        }
        #endregion
    }
}

