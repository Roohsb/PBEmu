using Core.server;

namespace Game.global.serverpacket
{
    public class FRIEND_ROOM_INVITE_PAK : SendPacket
    {
        private int _idx;
        public FRIEND_ROOM_INVITE_PAK(int idx)
        {
            _idx = idx;
        }

        public override void Write()
        {
            WriteH(277);
            WriteC((byte)_idx);
        }
    }
}