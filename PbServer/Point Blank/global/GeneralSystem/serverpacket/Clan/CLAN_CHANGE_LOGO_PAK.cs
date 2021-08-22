using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_CHANGE_LOGO_PAK : SendPacket
    {
        private uint _logo;
        public CLAN_CHANGE_LOGO_PAK(uint logo)
        {
            _logo = logo;
        }

        public override void Write()
        {
            WriteH(1371);
            WriteD(_logo);
        }
    }
}