using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_CREATE_INVITE_PAK : SendPacket
    {
        private int _clanId;
        private uint _erro;
        public CLAN_CREATE_INVITE_PAK(uint erro, int clanId)
        {
            _erro = erro;
            _clanId = clanId;
        }

        public override void Write()
        {
            WriteH(1317);
            WriteD(_erro);
            if (_erro == 0)
                WriteD(_clanId);
        }
    }
}