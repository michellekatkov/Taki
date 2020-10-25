using System;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientSocket client = new ClientSocket("10.0.0.7", 50000);
            client.ConnectSocket();


            //Thread recv = new Thread(client.ReceiveResponses);
            //recv.Start();
            string msg = "";

            List<Dictionary<string, string>> myCards = new List<Dictionary<string, string>>();

            while (true)
            {
                msg = Console.ReadLine(); // wait for user input

                if (msg == "exit")
                {
                    break;
                }
                string code = "create_game"; // request has code 'place_cards'
                Request request = new Request(code);
                request.arguments.Add("lobby_name", "itay the king");
                request.arguments.Add("player_name", "itay");
                request.arguments.Add("password", "1234");

                client.SendRequest(request);
                Console.ReadLine();
                Response response = client.ReceiveResponse();
                //Console.WriteLine(response);
                break;
            }
            Console.WriteLine();
            client.Disconnect();
            Console.ReadLine();
        }
    }
}
