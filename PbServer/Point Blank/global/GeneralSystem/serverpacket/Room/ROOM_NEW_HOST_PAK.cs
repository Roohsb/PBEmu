using Core.server;

namespace Game.global.serverpacket
{
    public class ROOM_NEW_HOST_PAK : SendPacket
    {
        private uint _slot;
        public ROOM_NEW_HOST_PAK(uint slot)
        {
            _slot = slot;
        }

        public override void Write()
        {
            WriteH(3873);
            WriteD(_slot);
        }
    }
}