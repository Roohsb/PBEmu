using Core.models.account.clan;
using Core.models.room;
using Core.server;
using Game.data.managers;
using Game.data.model;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class LOBBY_JOIN_ROOM_PAK : SendPacket
    {
        private uint erro;
        private Room room;
        private int slotId;
        private Account leader;
        public LOBBY_JOIN_ROOM_PAK(uint erro, Account player = null, Account leader = null)
        {
            this.erro = erro;
            if (player != null)
            {
                slotId = player._slotId;
                room = player._room;
                this.leader = leader;
            }
        }
        public override void Write()
        {
            WriteH(3082);
            if (erro == 0 && room != null && leader != null)
            {
                lock (room._slots)
                    WriteData();
            }
            else WriteD(erro);
        }
        private void WriteData()
        {
            List<Account> roomPlayers = room.GetAllPlayers();
            WriteD(room._roomId);
            WriteD(slotId);
            WriteD(room._roomId);
            WriteS(room.name, 23);
            WriteH((short)room.mapId);
            WriteC(room.stage4v4);
            WriteC(room.room_type);
            WriteC((byte)room._state);
            WriteC((byte)roomPlayers.Count);
            WriteC((byte)room.GetSlotCount());
            WriteC((byte)room._ping);
            WriteC(room.weaponsFlag);
            WriteC(room.random_map);
            WriteC(room.special);
            WriteS(leader.player_name, 33);
            WriteD(room.killtime);
            WriteC(room.limit);
            WriteC(room.seeConf);
            WriteH((short)room.autobalans);
            WriteS(room.password, 4);
            WriteC((byte)room.countdown.GetTimeLeft());
            WriteD(room._leader);
            for (int i = 0; i < 16; ++i)
            {
                SLOT slot = room._slots[i];
                Account player = room.GetPlayerBySlot(slot);
                if (player != null)
                {
                    Clan clan = ClanManager.GetClan(player.clanId);
                    WriteC((byte)slot.state);
                    WriteC((byte)player.GetRank());
                    WriteD(clan._id);
                    WriteD(player.clanAccess);
                    WriteC((byte)clan._rank);
                    WriteD(clan._logo);
                    WriteC((byte)player.pc_cafe >= 5 ? (byte)2 : (byte)player.pc_cafe);
                    WriteC((byte)player.tourneyLevel);
                    WriteD((uint)player.effects);
                    WriteS(clan._name, 17);
                    WriteD(0);
                    WriteC(31);
                }
                else
                {
                    WriteC((byte)slot.state);
                    WriteB(new byte[10]);
                    WriteD(4294967295);
                    WriteB(new byte[28]);
                }
            }
            WriteC((byte)roomPlayers.Count);
            for (int i = 0; i < roomPlayers.Count; i++)
            {
                Account ac = roomPlayers[i];
                WriteC((byte)ac._slotId);
                WriteC((byte)(ac.player_name.Length + 1));
                WriteS(ac.player_name, ac.player_name.Length + 1);
                WriteC((byte)ac.name_color);
            }
            if (room.IsBotMode())
            {
                WriteC(room.aiCount);
                WriteC(room.aiLevel);
            }
        }
    }
}