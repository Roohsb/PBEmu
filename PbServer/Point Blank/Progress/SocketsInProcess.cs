using System.Net.Sockets;

namespace Game
{
    public class SocketsInProcess
    {
        public int InstanceAcepted;
        public string Handler;
        public Socket Concurrent;
        public bool Desconected;
        public GameClient GetGameClient;
    }
}
