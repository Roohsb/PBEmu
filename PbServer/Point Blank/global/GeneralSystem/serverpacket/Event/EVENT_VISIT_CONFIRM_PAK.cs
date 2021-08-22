using Core.managers.events;
using Core.models.account.players;
using Core.models.enums.errors;
using Core.server;

namespace Game.global.serverpacket
{
    public class EVENT_VISIT_CONFIRM_PAK : SendPacket
    {
        private EventVisitModel _event;
        private PlayerEvent _pev;
        private uint _erro;
        public EVENT_VISIT_CONFIRM_PAK(EventErrorEnum erro, EventVisitModel ev, PlayerEvent pev)
        {
            _erro = (uint)erro;
            _event = ev;
            _pev = pev;
        }

        public override void Write()
        {
            WriteH(2662);
            WriteD(_erro);
            if (_erro == 0x80001504)
            {
                WriteD(_event.id);
                WriteC((byte)_pev.LastVisitSequence1);
                WriteC((byte)_pev.LastVisitSequence2);
                WriteH(0xFFFF);
                WriteD(_event.startDate);
            }
        }
    }
}