using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Client
{
    class ClientSocket
    {
        private byte[] bytes = new byte[1024];
        private byte[] messageByte;
        private IPAddress ipAddress;
        private IPEndPoint ipEndPoint;
        private int port, bytesSent, bytesReceived;
        private bool isClientConnected;
        private Socket sender;

        public ClientSocket(string ipAddress, int port)
        {
            this.ipAddress = IPAddress.Parse(ipAddress);
            this.port = port;
            this.ipEndPoint = new IPEndPoint(this.ipAddress, this.port);
            this.isClientConnected = false;
        }


        public void ConnectSocket()
        {
            if (this.isClientConnected) return;
            this.sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.sender.Connect(this.ipEndPoint);
            this.isClientConnected = true;
        }


        public void Disconnect()
        {
            if (!this.isClientConnected) return;
            this.sender.Shutdown(SocketShutdown.Both);
            this.sender.Close();
            this.isClientConnected = false;
        }


        public void SendRequest(Request message)
        {
            this.bytesSent = this.sender.Send(Encoding.ASCII.GetBytes(message.Serialize()));
        }


        public Response ReceiveResponse()
        {
            this.bytesReceived = sender.Receive(this.bytes);
            string message_json =  Encoding.ASCII.GetString(this.bytes, 0, this.bytesReceived);
            Response response = this.DeserializeResponse(message_json);
            return response;
        }


        public Response DeserializeResponse(string message_json)
        {
            Response message = JsonConvert.DeserializeObject<Response>(message_json);
            return message;
        }

    }
}
