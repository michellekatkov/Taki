using PlayingAlgorithm;
using Client;
using System.Threading;

namespace PlayingAlgorithm
{

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
                player.gameEnded = false;
                while (!player.gameEnded)
                {
                    player.ProcessServerMessage(sock.ReceiveResponse());
                }
            }
        }
    
}
