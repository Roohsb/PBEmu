using Core;
using Core.managers.server;
using Game.global.serverpacket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Game
{
    public class LoginManager
    {
        public static ServerConfig Config;
        public static Socket mainSocket;
        public static ConcurrentDictionary<string, SocketsInProcess> _lIstClient = new ConcurrentDictionary<string, SocketsInProcess>();
        public static List<GameClient> _loginQueue = new List<GameClient>();
        public static bool StartAuth()
        {
            try
            {
                Config = ServerConfigSyncer.GenerateConfig(Settings.configId);
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint Local = new IPEndPoint(IPAddress.Parse(Settings.IP_Jogo), Settings.authPort);
                mainSocket.Bind(Local);
                mainSocket.Listen(10);
                mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }
        }
        private static void AcceptCallback(IAsyncResult result)
        {
            Socket clientSocket = (Socket)result.AsyncState;
            try
            {
                Socket handler = clientSocket.EndAccept(result);
                if (handler != null)
                {
                    GameClient client = new GameClient(handler);
                    string endereco = client.GetIPAddress();
                    if (endereco == null)
                    {
                        Console.WriteLine("Error [0x2] A - IP");
                        goto MainProcess;
                    }
                    if (NextModel.IPAdress.Contains(endereco))
                        goto MainProcess;
                    else if (EstanciaSocket(new SocketsInProcess
                    {
                        Handler = endereco,
                        InstanceAcepted = 0,
                        GetGameClient = client,
                        Concurrent = client._client
                    }, endereco))
                        goto MainProcess;
                    else
                    {
                        if (client != null)
                            client.Start();
                        Thread.Sleep(3);
                        goto MainProcess;
                    }
                }
            }
            catch
            {
                SendDebug.SendInfo("[Failed Login Manager]");
            }
            MainProcess:
            mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
        }
        public static bool EstanciaSocket(SocketsInProcess model, string endereco)
        {
            if (!_lIstClient.ContainsKey(endereco))
                _lIstClient.TryAdd(endereco, model);
            else
            {
                SocketsInProcess list = _lIstClient[endereco];
                if (list.Handler == endereco)
                    list.InstanceAcepted = ++list.InstanceAcepted;
                if (list.InstanceAcepted > 40)
                {
                    if (!list.Desconected)
                    {
                        SendDebug.SendInfo("Successfully disconnected. accept: [" + list.InstanceAcepted + "], IP: [" + list.Handler + ":" + Settings.authPort + "]");
                    }
                    list.Desconected = true;
                    list.GetGameClient._client.Dispose();
                    return true;
                }
            }
            return false;
        }
        public static int EnterQueue(GameClient sck)
        {
            if (sck == null)
                return -1;
            lock (_loginQueue)
            {
                if (_loginQueue.Contains(sck))
                    return -1;
                _loginQueue.Add(sck);
                return _loginQueue.IndexOf(sck);
            }
        }
        public static bool RemoveBanned(GameClient sck)
        {
            try
            {
                if (_lIstClient.ContainsKey(sck.GetIPAddress()) && _lIstClient.TryGetValue(sck.GetIPAddress(), out SocketsInProcess inSocks) && inSocks.InstanceAcepted < 20)
                    return _lIstClient.TryRemove(sck.GetIPAddress(), out inSocks);
            }
            catch
            {
                Console.WriteLine("Error [0x1 A] Grave.");
            }
            return false;
        }
    }
}