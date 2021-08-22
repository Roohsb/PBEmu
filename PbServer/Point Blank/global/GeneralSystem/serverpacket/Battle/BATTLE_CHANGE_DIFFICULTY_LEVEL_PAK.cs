using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_CHANGE_DIFFICULTY_LEVEL_PAK : SendPacket
    {
        private Room room;
        public BATTLE_CHANGE_DIFFICULTY_LEVEL_PAK(Room room)
        {
            this.room = room;
        }

        public override void Write()
        {
            WriteH(3377);
            WriteC(room.IngameAiLevel);
            for (int i = 0; i < 16; i++)
                WriteD(room._slots[i].aiLevel); //Level atual da I.A
        }
    }
}