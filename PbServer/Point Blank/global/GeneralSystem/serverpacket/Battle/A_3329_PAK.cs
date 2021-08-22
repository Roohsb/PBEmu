using Core.server;

namespace Game.global.serverpacket
{
    public class A_3329_PAK : SendPacket
    {
        public A_3329_PAK()
        {
        }

        public override void Write()
        {
            WriteH(3330);
            WriteD(0);
        }
    }
}