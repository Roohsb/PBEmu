using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_ROOM_INFO_PAK : SendPacket
    {
        private Room room;
        private bool isBotMode;
        public BATTLE_ROOM_INFO_PAK(Room r)
        {
            room = r;
            if (room != null)
                isBotMode = room.IsBotMode();
        }
        public BATTLE_ROOM_INFO_PAK(Room r, bool isBotMode)
        {
            room = r;
            this.isBotMode = isBotMode;
        }
        public override void Write()
        {
            if (room == null)
                return;
            WriteH(3848);
            WriteD(room._roomId);
            WriteS(room.name, 23);
            WriteH((short)room.mapId);
            WriteC(room.stage4v4);
            WriteC(room.room_type);
            WriteC((byte)room._state);
            WriteC((byte)room.GetAllPlayers().Count);
            WriteC((byte)room.GetSlotCount());
            WriteC((byte)room._ping);
            WriteC(room.weaponsFlag);
            WriteC(room.random_map);
            WriteC(room.special);
            Account leader = room.GetLeader();
            WriteS(leader != null ? leader.player_name : "", 33);
            WriteD(room.killtime);
            WriteC(room.limit);
            WriteC(room.seeConf);
            WriteH((short)room.autobalans);
            if (isBotMode)
            {
                WriteC(room.aiCount);
                WriteC(room.aiLevel);
            }
        }
    }
}