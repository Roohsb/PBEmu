using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_REPLACE_NOTICE_PAK : SendPacket
    {
        private uint _erro;
        public CLAN_REPLACE_NOTICE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1363);
            WriteD(_erro);
        }
    }
}