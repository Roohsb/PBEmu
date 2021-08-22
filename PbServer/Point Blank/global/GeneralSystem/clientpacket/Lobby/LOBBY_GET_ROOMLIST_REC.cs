using Core;
using Core.managers;
using Core.models.account;
using Core.models.account.clan;
using Core.models.account.players;
using Core.models.enums;
using Core.server;
using Game.data.chat;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using Game.Progress;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_GET_ROOMLIST_REC : ReceiveGamePacket
    {
        public string Texto = "Parabéns, você recebeu 5000 em dinheiro por jogar 1 hora e 3 caixas / pontos";
        public LOBBY_GET_ROOMLIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Channel channel = p.GetChannel();
                if (channel != null)
                {
                    channel.RemoveEmptyRooms();
                    List<Room> rooms = channel._rooms;


                    Listcache.Salas = rooms.Count;
                    Listcache.pvps = 0;
                    for (int i = 0; i < rooms.Count; i++)
                        if (rooms[i].name.Contains("@pvp "))
                            Listcache.pvps += 1;


                    List<Account> waiting = channel.GetWaitPlayers();
                    if (Listcache.SysBotChannel)
                    {
                        waiting = BOTs_Channel_Waiting(waiting, channel);
                     //   rooms = BOTs_Room_Waiting(rooms, channel);
                    }



                    int Rpages = (int)Math.Ceiling(rooms.Count / 15d), Apages = (int)Math.Ceiling(waiting.Count / 10d);
                    if (p.LastRoomPage >= Rpages)
                        p.LastRoomPage = 0;
                    if (p.LastPlayerPage >= Apages)
                        p.LastPlayerPage = 0;
                    int roomsCount = 0, playersCount = 0;
                    byte[] roomsArray = GetRoomListData(p.LastRoomPage, ref roomsCount, rooms);
                    byte[] waitingArray = GetPlayerListData(p.LastPlayerPage, ref playersCount, waiting);
                    _client.SendPacket(new LOBBY_GET_ROOMLIST_PAK(rooms.Count, waiting.Count, p.LastRoomPage++, p.LastPlayerPage++, roomsCount, playersCount, roomsArray, waitingArray));
                  //  if (Settings.EnableTimeReal)
                  //  {
                       //if (NextCash(p))
                       //{
                       //    int cash = p._money += 5000;
                       //    if (PlayerManager.UpdateAccountCash(p.player_id, cash))
                       //    {
                       //        p.SendPacket(new AUTH_WEB_CASH_PAK(0, p._gp, p._money), false);
                       //        SEND_ITEM_INFO.LoadGoldCash(p);
                       //    }
                       //    p.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, p, new ItemsModel(1301240000, "NextCash Cupid", 1, (uint)new Random().Next(1, 10))));
                       //    p.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0));
                       //    p.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, p.GetSessionId(), 0, true, Texto));
                       //    SendDebug.SendInfo("[NextCash] o Jogador jogou por 1 hora, e ganhou premios PlayerID: [" + p.player_id + "] || PlayerName: [" + p.player_name + "]");
                       //    if (NextModel.NextCashON.ContainsKey(p.login))
                       //        NextModel.NextCashON[p.login] = DateTime.Now;
                       //}
                        //  if (NextMsg(p) && p.player_name != "")
                        //  {
                        //      string site = "", MeioDeComunicacao = "";
                        //      if (LoginManager.Config.ExitURL != null)
                        //          site = $"Just enter the website - {LoginManager.Config.ExitURL} \n";
                        //      if (Settings.MeioDeComunicacao != "SeuDiscordOuTs")
                        //          MeioDeComunicacao = Settings.MeioDeComunicacao + "\n";
                        //      using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK("Buy VIP and get Advantages! \n " +
                        //          site +
                        //          MeioDeComunicacao +
                        //          "This message automatically appears on your screen. \n " +
                        //          $"{LorenstudioSettings.ProjectName} always innovating!"))
                        //          p.SendPacket(packet);
                        //      SendDebug.SendInfo("[Next Message] Server sending message to the Player. [" + p.player_name + "]");
                        //      if (Listcache.NetMsg.ContainsKey(p.login))
                     //   Listcache.NetMsg[p.login] = DateTime.Now;
                      //  }
                   // }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[LOBBY_GET_ROOMLIST_REC] " + ex.ToString());
            }
        }
        public bool NextCash(Account Player)
        {
            if (Player.player_name == "" && Player.GetRank() > 50)
                return false;
            if (!Listcache.NextCashON.ContainsKey(Player.login))
                Listcache.NextCashON.Add(Player.login, Player.LastLobbyEnter);
            if (Listcache.NextCashON.TryGetValue(Player.login, out Player.LastLobbyEnter) && (DateTime.Now - Player.LastLobbyEnter).Seconds < 3600)
                return false;
            return true;
        }
        public bool NextMsg(Account Player)
        {
            if (Player.player_name == "")
                return false;
            if (Listcache.NetMsg.TryGetValue(Player.login, out Player.LastLobbyEnter) && (DateTime.Now - Player.LastLobbyEnter).Seconds < 1800)
                return false;
            if (!Listcache.NetMsg.ContainsKey(Player.login))
                Listcache.NetMsg.Add(Player.login, Player.LastLobbyEnter);
            return true;
        }
        private byte[] GetRoomListData(int page, ref int count, List<Room> list)
        {
            using SendGPacket p = new SendGPacket();
            for (int i = (page * 15); i < list.Count; i++)
            {
                WriteRoomData(list[i], p);
                if (++count == 15)
                    break;
            }
            return p.mstream.ToArray();
        }
        private void WriteRoomData(Room room, SendGPacket p)
        {
            int restrictions = 0;
            p.writeD(room._roomId);
            p.writeS(room.name, 23);
            p.writeH((short)room.mapId);
            p.writeC(room.stage4v4);
            p.writeC(room.room_type);
            p.writeC((byte)room._state);
         //    if(!room.isSalaBot)
         //   {
         //        p.writeC(15);
         //         p.writeC(15);
         //   } else
            p.writeC((byte)room.GetAllPlayers().Count);
            p.writeC((byte)room.GetSlotCount());
            p.writeC((byte)room._ping);
            p.writeC(room.weaponsFlag);
            if (room.random_map > 0) restrictions += 2;
            if (room.password.Length > 0) restrictions += 4;
            if (room.limit > 0 && room._state > 0) restrictions += 128;
            p.writeC((byte)restrictions);
            p.writeC(room.special);
        }
        private void WritePlayerData(Account pl, SendGPacket p)
        {
            Clan clan = ClanManager.GetClan(pl.clanId);
            p.writeD(pl.GetSessionId());
            p.writeD(clan._logo);
            p.writeS(clan._name, 17);
            p.writeH((short)pl.GetRank());
            p.writeS(pl.player_name, 33);
            p.writeC((byte)pl.name_color);
            p.writeC(31); //31
        }
        private byte[] GetPlayerListData(int page, ref int count, List<Account> list)
        {
            using SendGPacket p = new SendGPacket();
            for (int i = (page * 10); i < list.Count; i++)
            {
                WritePlayerData(list[i], p);
                if (++count == 10)
                    break;
            }
            return p.mstream.ToArray();
        }
        public List<Room> BOTs_Room_Waiting(List<Room> waiting, Channel channel)
        {
            try
            {
                List<Room> rooms = waiting;//Guarda Informação Geral da salas no Lobby
                List<Room> List = Bots_r(channel);
                for (int i = 0; i < List.Count; i++)
                    rooms.Add(List[i]); //AQUI SERÁ ADICIONADO OS BOTS
                return rooms;
            }
            catch
            {
                return waiting;
            }
        }
        public List<Account> BOTs_Channel_Waiting(List<Account> waiting, Channel channel)
        {
            try
            {
                List<Account> BotsAndPlayers = waiting; //Guarda Informação Geral dos Players no Lobby
                List<Account> list = Bots();
                for (int i = 0; i < list.Count; i++)
                    BotsAndPlayers.Add(list[i]); //AQUI SERÁ ADICIONADO OS BOTS
                return BotsAndPlayers;
            }
            catch
            {
                return waiting; //Players normal
            }
        }
        public List<Room> Bots_r(Channel channel)
        {
            List<Room> Bots = new List<Room>();
            for (int i = 1; i < 100; i++)
            {
                Bots.Add(new Room(i, channel)
                {
                    name = "@camp",
                    mapId = 1, //PortoAkana
                    room_type = 1, //DeathMath
                    stage4v4 = 0, 
                    _state = RoomState.Battle, //jogando
                    isSalaBot = true,
                    weaponsFlag = 1,
                    _ping = 5,
                    random_map = 0,
                    special = 0
                });
            }
            return Bots;
        }
        public List<Account> Bots()
        {
            List<Account> Bots = new List<Account>();
            Random r = new Random();
            lock (new object())
            {
                for (int i = 1; i < 51; i++) //50 Bots
                {
                    Bots.Add(new Account
                    {
                        clanId = r.Next(0, 10000),
                        _rank = r.Next(1, 50),
                        _money = 2000,
                        _exp = 10000,
                        _gp = 2000,
                        player_name = StringsIn.Bots_Nicks(r.Next(1, 50)),
                        _isOnline = true,
                        name_color = 0,
                        Session = new PlayerSession
                        {
                            _playerId = 50000 + i,
                            _sessionId = 50000 + (uint)i,
                        }
                    });
                }
            }
            return Bots;
        }
    }
}