using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class A_3428_PAK : SendPacket
    {
        private Room room;
        public A_3428_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3429);
            WriteD(room.room_type);
            WriteD((room.GetTimeByMask() * 60) - room.GetInBattleTime());
            if (room.room_type == 7)
            {
                WriteD(room.red_dino);
                WriteD(room.blue_dino);
            }
            else if (room.room_type == 1 || room.room_type == 8 || room.room_type == 13)
            {
                WriteD(room._redKills);
                WriteD(room._blueKills);
            }
            else
            {
                WriteD(room.red_rounds);
                WriteD(room.blue_rounds);
            }
        }
    }
}