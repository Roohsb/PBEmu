using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_TITLE_DETACH_PAK : SendPacket
    {
        private uint _erro;
        public BASE_TITLE_DETACH_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(2624);
            WriteD(_erro);
        }
    }
}