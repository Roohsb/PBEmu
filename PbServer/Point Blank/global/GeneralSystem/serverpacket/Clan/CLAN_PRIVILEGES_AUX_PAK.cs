using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_PRIVILEGES_AUX_PAK : SendPacket
    {
        public CLAN_PRIVILEGES_AUX_PAK()
        {
        }

        public override void Write()
        {
            WriteH(1342);
        }
    }
}