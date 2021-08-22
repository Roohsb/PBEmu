using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_RANK_UP_PAK : SendPacket
    {
        private int _rank, _allExp;
        public BASE_RANK_UP_PAK(int rank, int allExp)
        {
            _rank = rank;
            _allExp = allExp;
        }

        public override void Write()
        {
            WriteH(2614);
            WriteD(_rank);
            WriteD(_rank);
            WriteD(_allExp); //EXP
        }
    }
}