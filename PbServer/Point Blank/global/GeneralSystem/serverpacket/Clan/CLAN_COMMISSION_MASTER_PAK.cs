using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_COMMISSION_MASTER_PAK : SendPacket
    {
        private uint _erro;
        public CLAN_COMMISSION_MASTER_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1338);
            WriteD(_erro);
        }
    }
}