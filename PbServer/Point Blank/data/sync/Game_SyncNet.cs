using Core;
using Core.models.account;
using Core.models.enums;
using Core.models.enums.flags;
using Core.models.enums.friends;
using Core.models.enums.missions;
using Core.models.room;
using Core.models.servers;
using Core.server;
using Core.xml;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.client_side;
using Game.data.utils;
using Game.data.xml;
using Game.global.serverpacket;
using Game.Progress;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Game.data.sync
{
    public static class Game_SyncNet
    {
        public static uint SIO_UDP_CONNRESET = 0x80000000 | 0x18000000 | 12;
        private static DateTime LastSyncCount;
        public static UdpClient udp;
        public static string addr;
        public static void Start()
        {
            try
            {
                udp = new UdpClient(Settings.syncPort);
                udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
                new Thread(Read).Start();
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }
        public static void Read()
        {
            udp.BeginReceive(new AsyncCallback(Recv), null);
        }
        private static void Recv(IAsyncResult res)
        {
            byte[] received = new byte[0];
            IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 8000);
            if (res.IsCompleted)
                received = udp.EndReceive(res, ref RemoteIpEndPoint);
            try
            {
                addr = RemoteIpEndPoint.Address.ToString();
            }
            catch { }
            Thread.Sleep(5);
            new Thread(Read).Start();
            if (received.Length >= 6)
                LoadPacket(received);
        }
        private static void LoadPacket(byte[] buffer)
        {
            ReceiveGPacket p = new ReceiveGPacket(buffer);
            short opcode = p.ReadH();
            int pass = p.ReadD();
            if (pass != NextModel.Pass)
            {
                if (!NextModel.IPAdress.Contains(addr) && addr != null)
                    ProcessX.BloquearAbuso(addr);
                return;
            }
            try
            {
                switch (opcode)
                {
                    case 10: Net_Room_Pass_Portal.Load(p); break;
                    case 20: Net_Room_C4.Load(p); break;
                    case 30: Net_Room_Death.Load(p); break;
                    case 40: Net_Room_HitMarker.Load(p); break;
                    case 50: Net_Room_Sabotage_Sync.Load(p); break;
                    case 100:
                        {
                            Account player = AccountManager.GetAccount(p.ReadQ(), true);
                            if (player != null)
                            {
                                player.SendPacket(new AUTH_ACCOUNT_KICK_PAK(1));
                                player.SendPacket(new SERVER_MESSAGE_ERROR_PAK(0x80001000));
                                player.Close(1000);
                            }
                            break;
                        }
                    case 110:
                        {
                            int type = p.ReadC();
                            int isConnect = p.ReadC();
                            Account player = AccountManager.GetAccount(p.ReadQ(), 0);
                            if (player != null)
                            {
                                Account friendAcc = AccountManager.GetAccount(p.ReadQ(), true);
                                if (friendAcc != null)
                                {
                                    FriendState state = isConnect == 1 ? FriendState.Online : FriendState.Offline;
                                    if (type == 0)
                                    {
                                        int idx = -1;
                                        Friend friend = friendAcc.FriendSystem.GetFriend(player.player_id, out idx);
                                        if (idx != -1 && friend != null && friend.state == 0)
                                            friendAcc.SendPacket(new FRIEND_UPDATE_PAK(FriendChangeState.Update, friend, state, idx));
                                    }
                                    else friendAcc.SendPacket(new CLAN_MEMBER_INFO_CHANGE_PAK(player, state));
                                }
                            }
                            break;
                        }
                    case 130:
                        {
                            long playerId = p.ReadQ();
                            byte type = p.ReadC();
                            byte[] data = p.ReadB(p.ReadUH());
                            Account player = AccountManager.GetAccount(playerId, true);
                            if (player != null)
                            {
                                if (type == 0)
                                    player.SendPacket(data);
                                else
                                    player.SendCompletePacket(data);
                            }
                            break;
                        }
                    case 150:
                        {
                            int serverId = p.ReadD();
                            int count = p.ReadD();
                            GameServerModel gs = ServersXML.getServer(serverId);
                            if (gs != null)
                                gs._LastCount = count;
                            break;
                        }
                    case 160: Net_Clan_Sync.Load(p); break;
                    case 170: Net_Friend_Sync.Load(p); break;
                    case 180: Net_Inventory_Sync.Load(p); break;
                    case 190: Net_Player_Sync.Load(p); break;
                    case 210: Net_Clan_Servers_Sync.Load(p); break;
                    case 220: Net_Player_Sync.ExcptionIP(p); break;
                    case 240: Net_Player_Sync.Location(p); break;
                    default:
                        {
                            SendDebug.SendInfo("[Game_SyncNet] Connection type not found: " + opcode);
                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                Logger.Error("[Crash/Game_SyncNet] Type: " + opcode + "\r\n" + ex.ToString());
                if (p != null)
                    Logger.Error("COMP: " + BitConverter.ToString(p.GetBuffer()));
            }
        }
        /// <summary>
        /// Envia as informações com pedido de sincronia para o UDP.
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="effects"></param>
        /// <param name="room"></param>
        public static void SendUDPPlayerSync(Room room, SLOT slot, int type)
        {
            try
            {
                Account player = room.GetPlayerBySlot(slot._id);
                using SendGPacket pk = new SendGPacket();
                pk.writeH(10);
                pk.writeD(NextModel.Pass);
                pk.writeD(room.UniqueRoomId);
                pk.writeD((room.mapId * 16) + room.room_type);
                pk.writeQ(room.StartTick);
                pk.writeC((byte)type);
                pk.writeC((byte)room.rodada);
                pk.writeC((byte)slot._id);
                pk.writeC((byte)slot.spawnsCount);
                pk.writeC(BitConverter.GetBytes(slot._playerId)[0]);
                pk.writeC((byte)player.player_name.Length);
                pk.writeS(player.player_name, (byte)player.player_name.Length);
                if (type == 0 || type == 2)
                {
                    CupomEffects effects = room.CupomEffectsCheck(player);
                    WriteCharaInfo(pk, room, slot, effects);
                }

                SendPacket(pk.mstream.ToArray(), room.UDPServer.Connection);
            }
            catch
            {

            }
        }
        public static void SendHPMortalIndividual(Room room, SLOT sLOT, bool valor)
        {
            using SendGPacket pk = new SendGPacket();
            pk.writeH(40);
            pk.writeD(NextModel.Pass);
            pk.writeD(room.UniqueRoomId);
            pk.writeD((room.mapId * 16) + room.room_type);
            pk.writeC((byte)(valor == false ? 0 : 1));
            pk.writeC((byte)sLOT._id);
            SendPacket(pk.mstream.ToArray(), room.UDPServer.Connection);
        }
        public static void SendIPPrestart(Account p, IPEndPoint connection)
        {
            string IP = p.PublicIP.ToString();
            if (!Listcache._conn.Contains(IP))
            {
                using SendGPacket pk = new SendGPacket();
                pk.writeH(60);
                pk.writeD(NextModel.Pass);
                pk.writeC((byte)p.player_name.Length);
                pk.writeS(p.player_name, p.player_name.Length);
                pk.writeQ(p.player_id);
                pk.writeC((byte)IP.Length);
                pk.writeS(IP, IP.Length);
                pk.writeC((byte)p.dados.Length);
                pk.writeS(p.dados, p.dados.Length);
                SendPacket(pk.mstream.ToArray(), connection);
                Listcache._conn.Add(IP);
            }
        }
        public static void RemovePrestart(IPEndPoint connection)
        {
            using SendGPacket pk = new SendGPacket();
            pk.writeH(70);
            pk.writeD(NextModel.Pass);
            SendPacket(pk.mstream.ToArray(), connection);
        }
        public static void SendHPIndividual(Room room, SLOT sLOT, int HP)
        {
            using SendGPacket pk = new SendGPacket();
            pk.writeH(50);
            pk.writeD(NextModel.Pass);
            pk.writeD(room.UniqueRoomId);
            pk.writeD((room.mapId * 16) + room.room_type);
            pk.writeD(HP);
            pk.writeC((byte)sLOT._id);
            SendPacket(pk.mstream.ToArray(), room.UDPServer.Connection);
        }
        private static void WriteCharaInfo(SendGPacket pk, Room room, SLOT slot, CupomEffects effects)
        {
            int charaId;
            if ((RoomType)room.room_type == RoomType.Boss || (RoomType)room.room_type == RoomType.Cross_Counter)
            {
                if (room.rodada == 1 && slot._team == 1 ||
                    room.rodada == 2 && slot._team == 0)
                    charaId = room.rodada == 2 ? slot._equip._red : slot._equip._blue;
                else if (room.TRex == slot._id)
                    charaId = -1;
                else
                    charaId = slot._equip._dino;
            }
            else charaId = slot._team == 0 ? slot._equip._red : slot._equip._blue;
            int HPBonus = 0;
            if (effects.HasFlag(CupomEffects.Ketupat))
                HPBonus += 10;
            if (effects.HasFlag(CupomEffects.HP5))
                HPBonus += 5;
            if (effects.HasFlag(CupomEffects.HP10))
                HPBonus += 10;
            if (charaId == -1)
            {
                pk.writeC(255);
                pk.writeH(65535);
            }
            else
            {
                pk.writeC((byte)ComDiv.GetIdStatics(charaId, 2));
                pk.writeH((short)ComDiv.GetIdStatics(charaId, 4));
            }
            pk.writeC((byte)HPBonus);
            pk.writeC(effects.HasFlag(CupomEffects.C4SpeedKit));
        }
        public static void SendUDPRoundSync(Room room)
        {
            using SendGPacket pk = new SendGPacket();
            pk.writeH(30);
            pk.writeD(NextModel.Pass);
            pk.writeD(room.UniqueRoomId);
            pk.writeD((room.mapId * 16) + room.room_type);
            pk.writeC((byte)room.rodada);
            SendPacket(pk.mstream.ToArray(), room.UDPServer.Connection);
        }
        public static void SendLoginKickInfo(Account player)
        {
            int serverId = player._status.serverId;
            if (serverId != 255 && serverId != 0)
            {
                GameServerModel gs = ServersXML.getServer(serverId);
                if (gs == null)
                    return;

                using SendGPacket pk = new SendGPacket();
                pk.writeH(100);
                pk.writeD(NextModel.Pass);
                pk.writeQ(player.player_id);
                SendPacket(pk.mstream.ToArray(), gs.Connection);
            }
            else
                player.SetOnlineStatus(false);
        }
        public static GameServerModel GetServer(AccountStatus status) => GetServer(status.serverId);
        public static GameServerModel GetServer(int serverId)
        {
            if (serverId == 255 || serverId == Settings.serverId)
                return null;
            return ServersXML.getServer(serverId);
        }
        public static void UpdateGSCount(int serverId)
        {
            try
            {
                if ((DateTime.Now - LastSyncCount).TotalSeconds < 5)
                    return;

                LastSyncCount = DateTime.Now;
                int players = 0;
                for (int i = 0; i < ChannelsXML._channels.Count; i++)
                    players += ChannelsXML._channels[i]._players.Count;
                for (int i = 0; i < ServersXML._servers.Count; i++)
                {
                    GameServerModel gs = ServersXML._servers[i];
                    if (gs._id == serverId)
                        gs._LastCount = players;
                    else
                    {
                        using SendGPacket pk = new SendGPacket();
                        pk.writeH(150);
                        pk.writeD(NextModel.Pass);
                        pk.writeD(serverId);
                        pk.writeD(players);
                        SendPacket(pk.mstream.ToArray(), gs.Connection);
                    }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[Game_SyncNet.UpdateGSCount] " + ex.ToString());
            }
        }

        public static void SendBytes(long playerId, SendPacket sp, int serverId)
        {
            if (sp == null)
                return;
            GameServerModel gs = GetServer(serverId);
            if (gs == null)
                return;
            byte[] data = sp.GetBytes("Game_SyncNet.SendBytes");
            using SendGPacket pk = new SendGPacket();
            pk.writeH(130);
            pk.writeD(NextModel.Pass);
            pk.writeQ(playerId);
            pk.writeC(0);
            pk.writeH((ushort)data.Length);
            pk.writeB(data);
            SendPacket(pk.mstream.ToArray(), gs.Connection);
        }

        public static void SendCompleteBytes(long playerId, byte[] buffer, int serverId)
        {
            if (buffer.Length == 0)
                return;
            GameServerModel gs = GetServer(serverId);
            if (gs == null)
                return;

            using (SendGPacket pk = new SendGPacket())
            {
                pk.writeH(130);
                pk.writeD(NextModel.Pass);
                pk.writeQ(playerId);
                pk.writeC(1);
                pk.writeH((ushort)buffer.Length);
                pk.writeB(buffer);
                SendPacket(pk.mstream.ToArray(), gs.Connection);
            }
        }
        public static void SendPacket(byte[] data, IPEndPoint ip)
        {
            udp.Send(data, data.Length, ip);
        }
        public static void genDeath(Room room, SLOT killer, FragInfos kills, bool isSuicide)
        {
            bool isBotMode = room.IsBotMode();
            Net_Room_Death.RegistryFragInfos(room, killer, out int score, isBotMode, isSuicide, kills);
            if (isBotMode)
            {
                killer.Score += killer.killsOnLife + room.IngameAiLevel + score;
                if (killer.Score > ushort.MaxValue)
                {
                    killer.Score = 65535;
                    SendDebug.SendInfo("[PlayerId: " + killer._id + "] the maximum BOT score is reached.");
                }
                kills.Score = killer.Score;
            }
            else
            {
                killer.Score += score;
                AllUtils.CompleteMission(room, killer, kills, MISSION_TYPE.NA, 0);
                kills.Score = score;
            }
            using (BATTLE_DEATH_PAK packet = new BATTLE_DEATH_PAK(room, kills, killer, isBotMode))
                room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
            Net_Room_Death.EndBattleByDeath(room, killer, isBotMode, isSuicide);
        }
    }
}