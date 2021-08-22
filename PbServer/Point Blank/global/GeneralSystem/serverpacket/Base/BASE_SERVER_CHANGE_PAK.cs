using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_SERVER_CHANGE_PAK : SendPacket
    {
        private int error;
        public BASE_SERVER_CHANGE_PAK(int error)
        {
            this.error = error;
        }

        public override void Write()
        {
            WriteH(2578);
            WriteD(error);
            //error < 0 = STBL_IDX_EP_SERVER_FAIL_MOVE
        }
    }
}