using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class CLAN_CLIENT_CLAN_CONTEXT_PAK : SendPacket
    {
        private int clansCount;
        public CLAN_CLIENT_CLAN_CONTEXT_PAK(int count)
        {
            clansCount = count;
        }

        public override void Write()
        {
            WriteH(1452);
            WriteD(clansCount);
            WriteC(170);
            WriteH((ushort)Math.Ceiling(clansCount / 170d));
            WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
        }
    }
}