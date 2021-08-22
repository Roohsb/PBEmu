using Core.models.room;
using Core.server;

namespace Game.global.serverpacket
{
    public class VOTEKICK_START_PAK : SendPacket
    {
        private VoteKick vote;
        public VOTEKICK_START_PAK(VoteKick vote)
        {
            this.vote = vote;
        }

        public override void Write()
        {
            WriteH(3399);
            WriteC((byte)vote.creatorIdx);
            WriteC((byte)vote.victimIdx);
            WriteC((byte)vote.motive);
        }
    }
}