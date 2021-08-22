using Core;
using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_CREATE_REQUIREMENTS_PAK : SendPacket
    {
        public CLAN_CREATE_REQUIREMENTS_PAK()
        {
        }

        public override void Write()
        {
            WriteH(1417);
            WriteC((byte)Settings.minCreateRank);
            WriteD(Settings.minCreateGold);
        }
    }
}