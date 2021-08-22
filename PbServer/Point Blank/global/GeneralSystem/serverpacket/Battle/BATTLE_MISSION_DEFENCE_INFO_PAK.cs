using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_MISSION_DEFENCE_INFO_PAK : SendPacket
    {
        private Room room;
        public BATTLE_MISSION_DEFENCE_INFO_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3387);
            WriteH((ushort)room.Bar1);
            WriteH((ushort)room.Bar2);
            for (int i = 0; i < 16; i++)
                WriteH(room._slots[i].damageBar1);
            for (int i = 0; i < 16; i++)
                WriteH(room._slots[i].damageBar2);
        }
    }
}