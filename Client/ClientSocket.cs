using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Threading;

namespace Client
{
    class ClientSocket
    {
        private const int BufferSize = 1024;
        private byte[] buffer = new byte[BufferSize];

        private List<Response> responsesToHandle = new List<Response>(); // List of responses during the game to handle.
        private object locker = new object();
        private string ip;
        private int port, bytesSent, bytesReceived;
        private bool isClientConnected;
        private Socket sender;


        public ClientSocket(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.isClientConnected = false;
        }


        public void ConnectSocket()
        {
            if (this.isClientConnected) return;
            this.sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                this.sender.Connect(this.ip, this.port);
                this.isClientConnected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }


        public void Disconnect()
        {
            if (!this.isClientConnected) return;
            try
            {
                this.sender.Shutdown(SocketShutdown.Both);
                this.sender.Close();
                this.isClientConnected = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }


        public void HandleGame()
        {
            /* This function starts 2 functions as threads.
             * One function is responsible for receiving messages from the server
             * Second function is handling the responses
             */ 
            Thread clientReceiver = new Thread(this.ReceiveGameResponses);
            Thread clientHandler = new Thread(this.ResponsesHandler);
            clientReceiver.Start();
            clientHandler.Start();
        }


        public void ReceiveGameResponses()
        {
            // This function receives messages from the server during
            // the game, and adds it to the list of responses to handle
            string message_json;
            while (true)
            {
                this.bytesReceived = sender.Receive(this.buffer);
                message_json = Encoding.ASCII.GetString(this.buffer, 0, this.bytesReceived);
                AppendResponses(message_json);
            }
            
        }


        public void ResponsesHandler()
        {
            int numOfResponsesLeft;
            while (true)
            {
                if (this.responsesToHandle.Count > 0) // There are messages to handle
                {
                    numOfResponsesLeft = this.responsesToHandle.Count;
                    lock (locker)
                    {
                        while (numOfResponsesLeft > 0)
                        {
                            Response response = this.responsesToHandle[0]; // Get first response
                            Console.WriteLine("Handling response: " + response.ToString());
                            // TODO: Handle response
                            this.responsesToHandle.Remove(response);
                            numOfResponsesLeft--;
                        }
                    }
                }
            }
        }

        private void AppendResponses(string message_json)
        {
            // Receives a json string and adds Response objects to the list
            Stack<char> s = new Stack<char>();
            string totalMessage = "";
            bool inQuotes = false;
            foreach (char c in message_json)
            {
                if (c == '"' && !inQuotes) inQuotes = true;
                else if (c == '"' && inQuotes) inQuotes = false;
                else if (c == '{' && !inQuotes) s.Push(c);
                else if (c == '}' && !inQuotes)
                {
                    if (s.Count > 1) s.Pop(); // if not the last bracket
                    else if (s.Count == 1) // last bracket in the stack
                    {
                        s.Pop();
                        totalMessage += c;
                        lock (locker)
                        {
                            this.responsesToHandle.Add(DeserializeResponse(totalMessage));
                        }
                        totalMessage = ""; // reset message incase of another
                        continue;
                    }
                }
                totalMessage += c;
            }
        }


        public void SendRequest(Request message)
        {
            this.bytesSent = this.sender.Send(Encoding.ASCII.GetBytes(message.Serialize()));
        }


        public Response ReceiveResponse()
        {
            // Receives a single json string from server and returns the Response object.
            // Used in the pre-game phase.
            this.bytesReceived = sender.Receive(this.buffer);
            string message_json = Encoding.ASCII.GetString(this.buffer, 0, this.bytesReceived);
            return DeserializeResponse(message_json);
        }


        public Response DeserializeResponse(string message_json)
        {
            // Receives a json string and returns a Response object
            Response message = JsonConvert.DeserializeObject<Response>(message_json);
            return message;
        }
    }
}
