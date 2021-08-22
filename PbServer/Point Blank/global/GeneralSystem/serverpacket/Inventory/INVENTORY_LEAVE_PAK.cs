using Core.server;

namespace Game.global.serverpacket
{
    public class INVENTORY_LEAVE_PAK : SendPacket
    {
        private int erro, type;
        public INVENTORY_LEAVE_PAK(int erro, int type = 0)
        {
            this.erro = erro;
            this.type = type;
        }

        public override void Write()
        {
            WriteH(3590);
            WriteD(erro);
            if (erro < 0)
                WriteD(type);
        }
    }
}