
using Core.server;

namespace Game.global.Authentication
{
    public class BASE_RANK_AWARDS_PAK : SendPacket
    {
        public int rank = 0;
        public BASE_RANK_AWARDS_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2667);
            for (int i = 1; i < 52; i++)
            {
                WriteC((byte)(i));
                WriteD(0);
                WriteD(0);
                WriteD(0);
                WriteD(0);
            }
        }
    }
}