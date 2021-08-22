using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_MISSION_GENERATOR_INFO_PAK : SendPacket
    {
        private Room _room;
        public BATTLE_MISSION_GENERATOR_INFO_PAK(Room room)
        {
            _room = room;

        }

        public override void Write()
        {
            WriteH(3369);
            WriteH((ushort)_room.Bar1);
            WriteH((ushort)_room.Bar2);
            for (int i = 0; i < 16; i++)
                WriteH(_room._slots[i].damageBar1);
            for (int i = 0; i < 16; i++)
                WriteH(_room._slots[i].damageBar2);
            WriteD(5);
            WriteD(5);
            WriteD(5);
        }
    }
}