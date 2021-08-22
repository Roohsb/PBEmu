using Core.server;

namespace Game.global.serverpacket
{
    public class SHOP_TEST2_PAK : SendPacket
    {
        public SHOP_TEST2_PAK()
        {
        }
        public override void Write()
        {
            WriteH(567); //Não existe
            WriteD(0);
            WriteD(0);
            WriteD(0);
            WriteD(44); //356
        }
    }
}