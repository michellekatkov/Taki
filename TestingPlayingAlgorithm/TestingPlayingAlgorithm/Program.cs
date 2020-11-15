using System;
using System.Collections.Generic;
using PlayingAlgorithm;
using Client;
using System.Threading;

namespace TestingPlayingAlgorithm
{
    public class Team
    {
        public ClientSocket[] clients;
        public string[] tokens;
        public TakiSimulation[] sim;
        public Player[] players;
        PlayerThread[] playerThreads;
        public string[] names;

        public Team()
        {
            clients = new ClientSocket[4];
            tokens = new string[4];
            names = new string[4];
            sim = new TakiSimulation[4]; // this should be inside player
            players = new Player[4];
            playerThreads = new PlayerThread[4];
        }

        public void ReadBroadcast(int numConnections)
        {
            for (int k = 0; k < numConnections; k++)
            {
                Response rsp = clients[k].ReceiveResponse();
                Console.WriteLine("read broadcast: " + k.ToString() + " " + rsp);
            }
        }

        public void ByeTeam()
        {
            for (int i = 0; i < 4; i++)
            {
                clients[i].Disconnect();
            }
        }

        public class PlayerThread
        {
            public Player player;
            public ClientSocket sock;
            public string token;
            public Thread thread;

            public void SetParams(Player player, ClientSocket sock, string token)
            {
                this.player = player;
                this.sock = sock;
                this.token = token;
            }

            public void Run(object obj)
            {
                player.RegisterServer(sock, token);
                while (true)
                {
                    player.ProcessServerMessage(sock.ReceiveResponse());
                }
            }
        }

        public void LoadGame()
        {
            for (int i = 0; i < 4; i++)
            {
                playerThreads[i] = new PlayerThread();
                playerThreads[i].SetParams(new Player(), clients[i], tokens[i]);
                playerThreads[i].thread = new Thread( playerThreads[i].Run );
                playerThreads[i].thread.Name = names[i];
                playerThreads[i].thread.Start();
                //sim[i]= TakiSimulation.LoadGame( clients[i] );
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //ClientSocket client = new ClientSocket("104.156.225.184", 8080);
            //ClientSocket client = new ClientSocket("10.0.0.7", 50000);
            //ClientSocket client = new ClientSocket("127.0.0.1", 50000);
            //client.ConnectSocket();

            List<Dictionary<string, string>> myCards = new List<Dictionary<string, string>>();
           
            Team team = CreateTeam();
            team.LoadGame();
            team.clients[0].SendRequest(StartGameRequest(team.tokens[0]));
            //team.LoadGame();
            // Response response = team.clients[0].ReceiveResponse();
            //Console.WriteLine(team.clients[0].GetResponses());
            // team.ByeTeam();
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
            Console.WriteLine("start game jwt: " + jwt);
            string code = "start_game";
            Request request = new Request(code);
            request.arguments.Add("jwt", jwt);
            return request;
        }
        public static Team CreateTeam()
        {
            Team team = new Team();
            //ClientSocket client1 = new ClientSocket("104.156.225.184", 8080);
            ClientSocket client1 = new ClientSocket("127.0.0.1", 50000);
            client1.ConnectSocket();
            client1.SendRequest(CreateGameRequest("itay the king", "itay", "1234"));
            Response response = client1.ReceiveResponse();
            team.clients[0] = client1;
            team.tokens[0] = response.arguments["jwt"];

            long game_id = response.arguments["game_id"]; // if status success

            for (int k = 1; k < 4; k++)
            {
                ClientSocket client2 = new ClientSocket("127.0.0.1", 50000);
                //ClientSocket client2 = new ClientSocket("104.156.225.184", 8080);
                client2.ConnectSocket();
                team.names[k] = "michelle_" + k.ToString();
                client2.SendRequest(JoinGameRequest(game_id, team.names[k], "1234"));
                team.clients[k] = client2;
                response = client2.ReceiveResponse();
                team.ReadBroadcast(k);
                team.tokens[k] = response.arguments["jwt"];
                Console.WriteLine(response);
            }
            return team;
        }
    }
}
