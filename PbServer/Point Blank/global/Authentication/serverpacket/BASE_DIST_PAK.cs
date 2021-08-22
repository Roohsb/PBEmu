using Core.server;

namespace Game.global.Authentication
{
    public class BASE_DIST_PAK : SendPacket
    {
        public BASE_DIST_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2679);
            WriteD(8);
            WriteC(1);
            WriteC(5);

            WriteH(1); //versão jogo
            WriteH(15); //versão jogo
            WriteH(42); //versão jogo
            WriteH(5); //versão jogo

            WriteH(1012); //versão udp
            WriteH(12); //versão udp

            WriteC(5);
            WriteS("Mar  2 2017", 11);
            WriteD(0);
            WriteS("11:10:23", 8);
            WriteB(new byte[7]);
            WriteS("DIST", 4);
            WriteH(0);
        }
    }
}