using Core.server;

namespace Game.global.serverpacket
{
    public class ROOM_GET_NICKNAME_PAK : SendPacket
    {
        private int slotIdx, color;
        private string name;
        public ROOM_GET_NICKNAME_PAK(int slot, string name, int color)
        {
            slotIdx = slot;
            this.name = name;
            this.color = color;
        }

        public override void Write()
        {
            WriteH(3844);
            WriteD(slotIdx);
            if (slotIdx >= 0)
            {
                WriteS(name, 33);
                WriteC((byte)color);
            }
        }
    }
}