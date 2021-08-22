using Core.server;
using Core.xml;

namespace Game.global.serverpacket
{
    public class BASE_QUEST_COMPLETE_PAK : SendPacket
    {
        private int missionId, value;
        public BASE_QUEST_COMPLETE_PAK(int progress, Card card)
        {
            missionId = card._missionBasicId;
            if (card._missionLimit == progress)
                missionId += 240;
            value = progress;
        }

        public override void Write()
        {
            WriteH(2600);
            WriteC((byte)missionId);
            WriteC((byte)value);
        }
    }
}