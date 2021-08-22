using Core.models.room;
using Core.server;

namespace Game.global.serverpacket
{
    public class VOTEKICK_UPDATE_PAK : SendPacket
    {
        private VoteKick _k;
        public VOTEKICK_UPDATE_PAK(VoteKick vote)
        {
            _k = vote;
        }

        public override void Write()
        {
            WriteH(3407);
            WriteC((byte)_k.kikar);
            WriteC((byte)_k.deixar);
        }
    }
}