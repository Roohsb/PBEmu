using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class ROOM_CHANGE_INFO_PAK : SendPacket
    {
        private string _leader;
        private Room _room;
        public ROOM_CHANGE_INFO_PAK(Room room, string leader)
        {
            _room = room;
            _leader = leader;
        }

        public override void Write()
        {
            WriteH(3859);
            WriteS(_leader, 33);
            WriteD(_room.killtime);
            WriteC(_room.limit);
            WriteC(_room.seeConf);
            WriteH((short)_room.autobalans); //0 - nada | 1 - qnt. | 2 - rank
        }
    }
}