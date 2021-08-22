using Core.server;

namespace Game.global.Authentication
{
    public class SERVER_MESSAGE_ITEM_RECEIVE_PAK : SendPacket
    {
        public uint VALOR;
        public SERVER_MESSAGE_ITEM_RECEIVE_PAK(uint VALOR)
        {
            this.VALOR = VALOR;
        }
        public override void Write()
        {
            WriteH(2692);
            WriteD(VALOR);
        }
    }
}
