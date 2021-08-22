using Core.managers;
using Core.server;

namespace Game.global.serverpacket
{
    public class SHOP_GET_ITEMS_PAK : SendPacket
    {
        private int _tudo;
        private ShopData data;
        public SHOP_GET_ITEMS_PAK(ShopData data, int tudo)
        {
            this.data = data;
            _tudo = tudo;
        }
        public override void Write()
        {
            WriteH(525); //1111 itens por página
            WriteD(_tudo);
            WriteD(data.ItemsCount);
            WriteD(data.Offset);
            WriteB(data.Buffer);
            WriteD(44);
        }
    }
}