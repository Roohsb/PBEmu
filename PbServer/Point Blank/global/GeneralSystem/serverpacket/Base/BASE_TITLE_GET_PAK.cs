using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_TITLE_GET_PAK : SendPacket
    {
        private int _slots;
        private uint _erro;
        public BASE_TITLE_GET_PAK(uint erro, int slots)
        {
            _erro = erro;
            _slots = slots;
        }

        public override void Write()
        {
            WriteH(2620);
            WriteD(_erro);
            WriteD(_slots);
        }
    }
}