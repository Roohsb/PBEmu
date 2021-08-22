using Core;
using Core.managers.server;
using Core.server;
using Game.data.model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;

namespace Game
{
    public static class GameManager
    {
        public static Socket mainSocket;
        public static bool ServerIsClosed;
        public static ConcurrentDictionary<uint, GameClient> _socketList = new ConcurrentDictionary<uint, GameClient>();
        public static ConcurrentDictionary<string, SocketsInProcess> _lIstClient = new ConcurrentDictionary<string, SocketsInProcess>();
        public static List<GameClient> _loginQueue = new List<GameClient>();
        public static bool StartGame()
        {
            try
            {
                mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint Local = new IPEndPoint(IPAddress.Parse(Settings.IP_Jogo), Settings.gamePort);
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
                        Console.WriteLine("Error [0x3] G - IP");
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
                    }, endereco))//Conexão já estabelecida...
                        goto MainProcess;
                    else
                    {
                        if (client != null)
                            AddSocket(client);
                        Thread.Sleep(5);
                        goto MainProcess;
                    }
                    
                }
            }
            catch
            {
                SendDebug.SendInfo("[Failed Game Manager]");
            }
            MainProcess:
                mainSocket.BeginAccept(new AsyncCallback(AcceptCallback), mainSocket);
        }
        public static void AddSocket(GameClient sck)
        {
            uint idx = 0;
            while (true)
            {
                if (idx == uint.MaxValue)
                    break;
                uint valor = ++idx;
                if (!_socketList.ContainsKey(valor) && _socketList.TryAdd(valor, sck))
                {
                    sck.SessionId = valor;
                    sck.Start();
                    return;
                }
            }
            sck.Close(500, false);
        }
        public static bool RemoveSocket(GameClient sck, Account p)
        {
            try
            {
                if (sck == null || sck.SessionId == 0)
                    return false;
                if (_socketList.ContainsKey(sck.SessionId) && _socketList.TryGetValue(sck.SessionId, out sck))
                {
                    SendDebug.SendInfo(p.player_name + " foi desconectado com sucesso.");
                    return _socketList.TryRemove(sck.SessionId, out sck);
                }
            }
            catch
            {
                Console.WriteLine("Error [0x1] Grave. ");
            }
            return false;
        }
        public static bool RemoveBanned(GameClient sck)
        {
            try
            {
                if (_lIstClient.ContainsKey(sck.GetIPAddress()) && _lIstClient.TryGetValue(sck.GetIPAddress(), out SocketsInProcess InsSocks))
                    return _lIstClient.TryRemove(sck.GetIPAddress(), out InsSocks);
            }
            catch
            {
                Console.WriteLine("Error [0x2] Grave. ");
            }
            return false;
        }
        public static bool EstanciaSocket(SocketsInProcess model, string endereco)
        {
            try
            {
                if (!_lIstClient.ContainsKey(endereco))
                    _lIstClient.TryAdd(endereco, model);
                else
                {
                    SocketsInProcess list = _lIstClient[endereco];
                    if (list.Handler == endereco)
                        list.InstanceAcepted = ++list.InstanceAcepted;
                    if (list.InstanceAcepted > 20)
                    {
                        if (!list.Desconected)
                            SendDebug.SendInfo("Desconectado com sucesso.  Aceitar: [" + list.InstanceAcepted + "], IP: [" + list.Handler + ":" + Settings.gamePort + "]");
                        list.Desconected = true;
                        list.GetGameClient.Close(1000);
                        ProcessX.BloquearAbuso(endereco);
                        return true;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Erro de registro de valor na Lista Negra, entre em contato com o desenvolvedor.");
            }
            return false;
        }
        public static int SendPacketToAllClients(SendPacket packet)
        {
            int count = 0;
            if (_socketList.Count == 0)
                return count;
            byte[] data = packet.GetCompleteBytes("GameManager.SendPacketToAllClients");
            foreach (var player in from GameClient client in _socketList.Values let player = client._player where player != null && player._isOnline select player)
            {
                player.SendCompletePacket(data);
                count++;
            }
            return count;
        }
        public static void EventsReloadsTrue()
        {
            foreach (var player in from GameClient client in _socketList.Values let player = client._player where player != null && player._isOnline select player)
                player.quiz = true;
        }
        public static void EventsReloadsFalse()
        {
            foreach (var player in from GameClient client in _socketList.Values let player = client._player where player != null && player._isOnline select player)
                        player.quiz = false;
        }
        public static Account PlayerOnlines()
        {
            if (_socketList.Count == 0)
                return null;
            foreach (Account player in from GameClient client in _socketList.Values let player = client._player where player != null && player._isOnline select player)
                 return player;
            return null;
        }
        public static int PlayersOnlines()
        {
            int players = 0;
            if (_socketList.Count == 0)
                return 0;
            foreach (Account player in from GameClient client in _socketList.Values let player = client._player where player != null && player._isOnline select player)
            {
                players += 1;
            }
            return players;
        }
        public static void ClearGC()
        {
            if (_socketList.Count == 0)
                return;
            int count = 0;
            foreach(GameClient game in _socketList.Values)
            {
                if(game != null && game.player_id == 0 && game._player == null)
                {
                    bool gc = _socketList.TryRemove(game.SessionId, out GameClient games);
                    if(gc)
                    {
                        count += 1;
                    }
                }
            }
            Console.WriteLine("Removed: " + string.Concat(count) + " socket / s with no direct connection to the server.");
        }
        public static int KickActiveClient(double Hours)
        {
            int count = 0;
            foreach (Account pl in from GameClient client in _socketList.Values  let pl = client._player where pl != null && pl._room == null && pl.channelId > -1 && !pl.IsGM() && (DateTime.Now - pl.LastLobbyEnter).TotalHours >= Hours select pl)
            {
                count++;
                pl.Close(5000);
            }
            return count;
        }
        public static int KickCountActiveClient(double Hours)
        {
            int count = 0;
            count += (from GameClient client in _socketList.Values let pl = client._player where pl != null && pl._room == null && pl.channelId > -1 && !pl.IsGM() && (DateTime.Now - pl.LastLobbyEnter).TotalHours >= Hours select client).Count();
            return count;
        }
    }
}