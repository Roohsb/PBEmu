using Core.models.enums.errors;
using Core.server;

namespace Game.global.serverpacket
{
    public class EVENT_VISIT_REWARD_PAK : SendPacket
    {
        private uint _erro;
        public EVENT_VISIT_REWARD_PAK(EventErrorEnum erro)
        {
            _erro = (uint)erro;
        }

        public override void Write()
        {
            WriteH(2664);
            WriteD(_erro);
        }
    }
}