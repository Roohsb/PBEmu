using Core.managers;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class BOX_MESSAGE_GIFT_TAKE_PAK : SendPacket
    {
        private uint _erro;
        private List<ItemsModel> charas = new List<ItemsModel>(),
            weapons = new List<ItemsModel>(),
            cupons = new List<ItemsModel>();
        public BOX_MESSAGE_GIFT_TAKE_PAK(uint erro, ItemsModel item = null, Account p = null)
        {
            _erro = erro;
            if (_erro == 1)
                get(item, p);
        }

        public override void Write()
        {
            WriteH(541);
            WriteD(_erro); //2231369729 - erro | 1 - sucesso
            if (_erro == 1)
            {
                WriteD(charas.Count);
                WriteD(weapons.Count);
                WriteD(cupons.Count);
                WriteD(0);
                for (int i = 0; i < charas.Count; i++)
                {
                    ItemsModel item = charas[i];
                    WriteQ(item._objId);
                    WriteD(item._id);
                    WriteC((byte)item._equip);
                    WriteD(item._count);
                }
                for (int i = 0; i < weapons.Count; i++)
                {
                    ItemsModel item = weapons[i];
                    WriteQ(item._objId);
                    WriteD(item._id);
                    WriteC((byte)item._equip);
                    WriteD(item._count);
                }
                for (int i = 0; i < cupons.Count; i++)
                {
                    ItemsModel item = cupons[i];
                    WriteQ(item._objId);
                    WriteD(item._id);
                    WriteC((byte)item._equip);
                    WriteD(item._count);
                }
            }
        }

        private void get(ItemsModel item, Account p)
        {
            try
            {
                ItemsModel modelo = new ItemsModel(item) { _objId = item._objId };
                PlayerManager.TryCreateItem(modelo, p._inventory, p.player_id);
                switch (modelo._category)
                {
                    case 1: weapons.Add(modelo); break;
                    case 2: charas.Add(modelo); break;
                    case 3: cupons.Add(modelo); break;
                }
            }
            catch
            { p.Close(0); }
        }
    }
}