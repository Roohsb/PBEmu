using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_MATCH_TEAM_CONTEXT_PAK : SendPacket
    {
        private int count;
        public CLAN_WAR_MATCH_TEAM_CONTEXT_PAK(int count)
        {
            this.count = count;
        }

        public override void Write()
        {
            WriteH(1543);
            WriteH((short)count);
            WriteC(13);
            WriteH((short)Math.Ceiling(count / 13d));
        }
    }
}