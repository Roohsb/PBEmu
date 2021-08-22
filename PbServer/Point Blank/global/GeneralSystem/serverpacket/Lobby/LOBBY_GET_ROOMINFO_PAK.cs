using Core;
using Core.server;
using Game.data.model;
using System;

namespace Game.global.serverpacket
{
    public class LOBBY_GET_ROOMINFO_PAK : SendPacket
    {
        private Room room;
        private Account leader;
        public LOBBY_GET_ROOMINFO_PAK(Room room, Account leader)
        {
            this.room = room;
            this.leader = leader;
        }

        public override void Write()
        {
            if (room == null || leader == null)
                return;
            WriteH(3088);
            try
            {
                WriteS(leader.player_name, 33);
                WriteC((byte)room.killtime);
                WriteC((byte)(room.rodada - 1));
                WriteH((ushort)room.GetInBattleTime());
                WriteC(room.limit);
                WriteC(room.seeConf);
                WriteH((ushort)room.autobalans);
            }
            catch (Exception ex)
            {
                WriteS("", 33);
                WriteB(new byte[8]);
                SendDebug.SendInfo("[LOBBY_GET_ROOMINFO_PAK] " + ex.ToString());
            }
        }
    }
}