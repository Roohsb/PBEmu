using Core.server;
using Game.data.model;
using Game.data.utils;

namespace Game.global.serverpacket
{
    public class BATTLE_TIMERSYNC_PAK : SendPacket
    {
        private Room _r;
        public BATTLE_TIMERSYNC_PAK(Room r)
        {
            _r = r;
        }

        public override void Write()
        {
            WriteH(3371);
            WriteC((byte)_r.rodada);
            WriteD(_r.GetInBattleTimeLeft());
            WriteH(AllUtils.getSlotsFlag(_r, true, false));
        }
    }
}