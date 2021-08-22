using Core.models.account.players;
using Core.server;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_INVENTORY_PAK : SendPacket
    {
        private List<ItemsModel> charas = new List<ItemsModel>(),weapons = new List<ItemsModel>(),cupons = new List<ItemsModel>();
        public BASE_USER_INVENTORY_PAK(List<ItemsModel> items)
        {
            InventoryLoad(items);
        }
        private void InventoryLoad(List<ItemsModel> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemsModel item = items[i];
                switch(item._category)
                {
                    case 1: weapons.Add(item); break;
                    case 2: charas.Add(item); break;
                    case 3: cupons.Add(item); break;
                }
            }
        }
        public override void Write()
        {
            WriteH(2699);
            WriteD(charas.Count);
            for (int i = 0; i < charas.Count; i++)
            {
                ItemsModel item = charas[i];
                WriteQ(item._objId);
                WriteD(item._id);
                WriteC((byte)item._equip);
                WriteD(item._count);
            }
            WriteD(weapons.Count);
            for (int i = 0; i < weapons.Count; i++)
            {
                ItemsModel item = weapons[i];
                WriteQ(item._objId);
                WriteD(item._id);
                WriteC((byte)item._equip);
                WriteD(item._count);
            }
            WriteD(cupons.Count);
            for (int i = 0; i < cupons.Count; i++)
            {
                ItemsModel item = cupons[i];
                WriteQ(item._objId);
                WriteD(item._id);
                WriteC((byte)item._equip);
                WriteD(item._count);
            }
            WriteD(0);
            WriteD(0);
        }
    }
}