using Core.managers.events;
using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_PLAYED_TIME_PAK : SendPacket
    {
        private int _type;
        private PlayTimeModel ev;
        public BATTLE_PLAYED_TIME_PAK(int type, PlayTimeModel eventPt)
        {
            _type = type;
            ev = eventPt;
        }

        public override void Write()
        {
            WriteH(3911);
            WriteC((byte)_type);
            WriteS(ev._title, 30);
            WriteD(ev._goodReward1);
            WriteD(ev._goodReward2);
            WriteQ(ev._time);
        }
    }
}