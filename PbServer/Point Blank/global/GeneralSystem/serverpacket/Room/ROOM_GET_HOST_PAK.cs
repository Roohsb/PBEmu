using Core.server;

namespace Game.global.serverpacket
{
    public class ROOM_GET_HOST_PAK : SendPacket
    {
        private uint _slot;
        public ROOM_GET_HOST_PAK(uint slot)
        {
            _slot = slot;
        }
        public ROOM_GET_HOST_PAK(int slot)
        {
            _slot = (uint)slot;
        }
        public override void Write()
        {
            WriteH(3867);
            WriteD(_slot);
        }
    }
}