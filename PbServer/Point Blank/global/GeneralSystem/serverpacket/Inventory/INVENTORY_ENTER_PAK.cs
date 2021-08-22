using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class INVENTORY_ENTER_PAK : SendPacket
    {
        public INVENTORY_ENTER_PAK()
        {
        }

        public override void Write()
        {
            WriteH(3586);
            WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
        }
    }
}