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
    class Team
    {
        public ClientSocket[] clients;
        public string[] tokens;
        public Team()
        {
            clients = new ClientSocket[4];
            tokens = new string[4];
        }

        public void ReadBroadcast()
        {

        }

        public void ByeTeam()
        {
            for (int i = 0; i < 4; i++)
            {
                clients[i].Disconnect();
            }

        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Team team = CreateTeam();
            team.clients[0].HandleGame();
            team.clients[0].SendRequest(StartGameRequest(team.tokens[0]));
            Console.ReadLine(); // Wait before closing sockets.
            team.ByeTeam();
        }
        public static Request CreateGameRequest(string lobby_name, string player_name, string password)
        {
            string code = "create_game"; // request has code 'place_cards'
            Request request = new Request(code);
            request.arguments.Add("lobby_name", lobby_name);
            request.arguments.Add("player_name", player_name);
            request.arguments.Add("password", password);
            return request;
        }
        public static Request JoinGameRequest(long game_id, string player_name, string password)
        {
            string code = "join_game";
            Request request = new Request(code);
            request.arguments.Add("game_id", game_id);
            request.arguments.Add("player_name", player_name);
            request.arguments.Add("password", password);
            return request;
        }
        public static Request LeaveGameRequest()
        {
            string code = "leave_game";
            Request request = new Request(code);
            request.arguments.Add("jwt", "");
            return request;
        }
        public static Request StartGameRequest(string jwt)
        {
            string code = "start_game";
            Request request = new Request(code);
            request.arguments.Add("jwt", jwt);
            return request;
        }
        public static Team CreateTeam()
        {
            Team team = new Team();
            ClientSocket client1 = new ClientSocket("104.156.225.184", 8080);
            client1.ConnectSocket();
            client1.SendRequest(CreateGameRequest("lobby1", "itay", "1234"));
            Response response = client1.ReceiveResponse();
            team.clients[0] = client1;
            team.tokens[0] = response.arguments["jwt"];

            long game_id = response.arguments["game_id"]; // if status success

            for (int k = 1; k < 4; k++)
            {
                ClientSocket client2 = new ClientSocket("104.156.225.184", 8080);
                client2.ConnectSocket();
                client2.SendRequest(JoinGameRequest(game_id,
                    "michelle_" + k.ToString(), "1234"));
                team.clients[k] = client2;
                response = client2.ReceiveResponse();
                team.tokens[k] = response.arguments["jwt"];
                //Console.WriteLine(response);
            }
            return team;
        }
    }
}
