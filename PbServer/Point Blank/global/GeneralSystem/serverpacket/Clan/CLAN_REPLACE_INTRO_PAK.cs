using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_REPLACE_INTRO_PAK : SendPacket
    {
        private uint _erro;
        public CLAN_REPLACE_INTRO_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1365);
            WriteD(_erro);
        }
    }
}