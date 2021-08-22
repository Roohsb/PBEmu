using Core.server;

namespace Game.global.serverpacket
{
    public class SHOP_LEAVE_PAK : SendPacket
    {
        public SHOP_LEAVE_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2818);
        }
    }
}