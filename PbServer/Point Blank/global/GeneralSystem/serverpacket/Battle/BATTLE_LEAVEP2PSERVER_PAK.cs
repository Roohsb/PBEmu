using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_LEAVEP2PSERVER_PAK : SendPacket
    {
        private Account p;
        private int type;
        public BATTLE_LEAVEP2PSERVER_PAK(Account p, int type)
        {
            this.p = p;
            this.type = type;
        }

        public override void Write()
        {
            if (p == null)
                return;
            WriteH(3385);
            WriteD(p._slotId);
            WriteC((byte)type);
            WriteD(p._exp);
            WriteD(p._rank);
            WriteD(p._gp);
            WriteD(p._statistic.escapes);
            WriteD(p._statistic.escapes);
        }
    }
}