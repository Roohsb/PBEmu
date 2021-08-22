using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_TUTORIAL_ROUND_END_PAK : SendPacket
    {
        public BATTLE_TUTORIAL_ROUND_END_PAK()
        {
        }

        public override void Write()
        {
            WriteH(3395);
            WriteC(3);
            WriteD(110);
        }
    }
}