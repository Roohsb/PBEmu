using Core.server;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_ITEM_RECEIVE_PAK : SendPacket
    {
        private uint _er;
        public SERVER_MESSAGE_ITEM_RECEIVE_PAK(uint er)
        {
            _er = er;
        }

        public override void Write()
        {
            WriteH(2692);
            WriteD(_er);
        }
    }
}