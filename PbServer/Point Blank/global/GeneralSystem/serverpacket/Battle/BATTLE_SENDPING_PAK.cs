using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_SENDPING_PAK : SendPacket
    {
        public BATTLE_SENDPING_PAK()
        {
        }

        public override void Write()
        {
            WriteH(3345);
            for(int i = 0; i < 16; ++i)
                WriteC(5);
        }
    }
}