using Core.managers;
using Core.server;
using System;

namespace Game.global.serverpacket
{
    public class CLAN_REQUEST_CONTEXT_PAK : SendPacket
    {
        private uint _erro;
        private int invites;
        public CLAN_REQUEST_CONTEXT_PAK(int clanId)
        {
            if (clanId > 0)
                invites = PlayerManager.GetRequestCount(clanId);
            else
                _erro = 4294967295;
        }

        public override void Write()
        {
            WriteH(1321);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteC((byte)invites);
                WriteC(13);
                WriteC((byte)Math.Ceiling(invites / 13d));
                WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            }
        }
    }
}