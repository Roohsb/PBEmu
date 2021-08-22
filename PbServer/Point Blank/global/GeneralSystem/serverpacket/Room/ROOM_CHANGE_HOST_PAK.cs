using Core.server;

namespace Game.global.serverpacket
{
    public class ROOM_CHANGE_HOST_PAK : SendPacket
    {
        private uint _slot;
        public ROOM_CHANGE_HOST_PAK(uint slot)
        {
            _slot = slot;
        }
        public ROOM_CHANGE_HOST_PAK(int slot)
        {
            _slot = (uint)slot;
        }
        public override void Write()
        {
            WriteH(3871);
            WriteD(_slot);
        }
    }
}