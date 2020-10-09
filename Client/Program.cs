using System;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            ClientSocket client = new ClientSocket("10.0.0.14", 50000);
            client.ConnectSocket();

            string msg = "";

            List<Dictionary<string, string>> myCards = new List<Dictionary<string, string>>();

            while (msg != "exit")
            {
                msg = Console.ReadLine(); // wait for user input

                string code = "take_cards"; // request has code 'place_cards'
                string jwt = "Random json-web-token";
                Request request = new Request(code);
                request.jwt = jwt;

                client.SendRequest(request);
                Response response = client.ReceiveResponse();

                Console.WriteLine(response.code);

                List<Dictionary<string, string>> cardsList = response.arguments["cards"].ToObject(typeof(List<Dictionary<string, string>>)); // convert to .NET object
                foreach(Dictionary<string,string> card in cardsList)
                {
                    myCards.Add(card);
                }


                foreach(Dictionary<string,string> card in myCards)
                {
                    foreach (KeyValuePair<string,string> kvp in card)
                    {
                        Console.WriteLine("{0}: {1}", kvp.Key, kvp.Value);
                    }
                    Console.WriteLine();
                }

                msg = "exit";
            }
            client.Disconnect();
            Console.ReadLine();
        }
    }
}
