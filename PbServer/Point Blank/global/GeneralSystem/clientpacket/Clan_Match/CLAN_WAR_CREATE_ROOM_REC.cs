using Core;
using Core.models.enums;
using Core.models.enums.match;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_CREATE_ROOM_REC : ReceiveGamePacket
    {
        private Match MyMatch, EnemyMatch;
        private int roomId = -1;
        public CLAN_WAR_CREATE_ROOM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            Account p = _client._player;
            if (p == null || p.clanId == 0)
                return;
            Channel channel = p.GetChannel();
            MyMatch = p._match;
            if (channel == null || MyMatch == null)
                return;
            int match = ReadH();
            ReadB(8);
            EnemyMatch = channel.GetMatch(match);
            try
            {
                if (EnemyMatch == null)
                    return;
                lock (channel._rooms)
                    for (int i = 0; i < 300; i++)
                        if (channel.GetRoom(i) == null)
                        {
                            Room room = new Room(i, channel);
                            ReadH();
                            room.name = ReadS(23);
                            room.mapId = ReadH();
                            room.stage4v4 = ReadC();
                            room.room_type = ReadC();
                            ReadH();
                            room.InitSlotCount(ReadC());
                            ReadC();
                            room.weaponsFlag = ReadC();
                            room.random_map = ReadC();
                            room.special = ReadC();
                            room.password = "";
                            room.killtime = 3;
                            room.AddPlayer(p);
                            channel.AddRoom(room);
                            _client.SendPacket(new LOBBY_CREATE_ROOM_PAK(0, room, p));
                            roomId = i;
                            return;
                        }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public override void Run()
        {
            if (roomId == -1)
                return;
            using (CLAN_WAR_ENEMY_INFO_PAK packet = new CLAN_WAR_ENEMY_INFO_PAK(EnemyMatch))
            using (CLAN_WAR_JOINED_ROOM_PAK packet2 = new CLAN_WAR_JOINED_ROOM_PAK(EnemyMatch, roomId, 0))
            {
                System.Collections.Generic.List<Account> list = MyMatch.GetAllPlayers(MyMatch._leader);
                byte[] data1 = packet.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REC-1");
                byte[] data2 = packet2.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REC-2");
                for (int i = 0; i < list.Count; i++)
                {
                    Account pM = list[i];
                    if (pM._match != null)
                    {
                        pM.SendCompletePacket(data1);
                        pM.SendCompletePacket(data2);
                        MyMatch._slots[pM.matchSlot].state = SlotMatchState.Ready;
                    }
                }
            }
            using (CLAN_WAR_ENEMY_INFO_PAK packet = new CLAN_WAR_ENEMY_INFO_PAK(MyMatch))
            using (CLAN_WAR_JOINED_ROOM_PAK packet2 = new CLAN_WAR_JOINED_ROOM_PAK(MyMatch, roomId, 1))
            {
                System.Collections.Generic.List<Account> list = EnemyMatch.GetAllPlayers();
                byte[] data1 = packet.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REC-3");
                byte[] data2 = packet2.GetCompleteBytes("CLAN_WAR_CREATE_ROOM_REC-4");
                for (int i = 0; i < list.Count; i++)
                {
                    Account pM = list[i];
                    if (pM._match != null)
                    {
                        pM.SendCompletePacket(data1);
                        pM.SendCompletePacket(data2);
                        MyMatch._slots[pM.matchSlot].state = SlotMatchState.Ready;
                    }
                }
            }
        }
    }
}