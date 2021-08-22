using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class SHOP_LIST_PAK : SendPacket
    {
        public SHOP_LIST_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2822);
            WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
        }
    }
}