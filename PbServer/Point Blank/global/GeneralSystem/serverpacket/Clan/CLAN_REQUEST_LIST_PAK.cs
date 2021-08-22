using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_REQUEST_LIST_PAK : SendPacket
    {
        private int erro, page, count;
        private byte[] array;
        public CLAN_REQUEST_LIST_PAK(int erro, int count, int page, byte[] array)
        {
            this.erro = erro;
            this.count = count;
            this.page = page;
            this.array = array;
        }
        public CLAN_REQUEST_LIST_PAK(int erro)
        {
            this.erro = erro;
        }
        public override void Write()
        {
            WriteH(1323);
            WriteD(erro);
            if (erro >= 0)
            {
                WriteC((byte)page);
                WriteC((byte)count);
                WriteB(array);
            }
        }
    }
}