using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class SHOP_ENTER_PAK : SendPacket
    {
        public SHOP_ENTER_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2820);
            WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
        }
    }
}