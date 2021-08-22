using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class LOBBY_CREATE_ROOM_PAK : SendPacket
    {
        private Account leader;
        private Room room;
        private uint erro;
        public LOBBY_CREATE_ROOM_PAK(uint err, Room r, Account p)
        {
            erro = err;
            room = r;
            leader = p;
        }

        public override void Write()
        {
            WriteH(3090);
            WriteD(erro == 0 ? (uint)room._roomId : erro);
            if (erro == 0)
            {
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
                WriteS(leader.player_name, 33);
                WriteD(room.killtime);
                WriteC(room.limit);
                WriteC(room.seeConf);
                WriteH((short)room.autobalans);
            }
        }
    }
}