using Core;
using Core.managers;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using Game.data.sync.server_side;
using System;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class INVENTORY_ITEM_CREATE_PAK : SendPacket
    {
        private int _type;
        private List<ItemsModel> weapons = new List<ItemsModel>(),
            charas = new List<ItemsModel>(),
            cupons = new List<ItemsModel>();

        public INVENTORY_ITEM_CREATE_PAK(int type, Account player, List<ItemsModel> items)
        {
            _type = type;
            AddItems(player, items);
        }
        public INVENTORY_ITEM_CREATE_PAK(int type, Account player, ItemsModel item)
        {
            _type = type;
            AddItems(player, item);
        }
        public override void Write()
        {
            WriteH(3588);
            WriteC((byte)_type);
            WriteD(charas.Count);
            WriteD(weapons.Count);
            WriteD(cupons.Count);
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

        private void AddItems(Account p, List<ItemsModel> items)
        {
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemsModel item = items[i];
                    ItemsModel modelo = new ItemsModel(item) { _objId = item._objId };
                    if (_type == 1)
                        PlayerManager.TryCreateItem(modelo, p._inventory, p.player_id);
                    SEND_ITEM_INFO.LoadItem(p, modelo);
                    switch(modelo._category)
                    {
                        case 1: weapons.Add(modelo); break;
                        case 2: charas.Add(modelo); break;
                        case 3: cupons.Add(modelo); break;
                    }
                }
            }
            catch (Exception ex)
            {
                p.Close(0);
                SendDebug.SendInfo("[INVENTORY_ITEM_CREATE_PAK1] " + ex.ToString());
            }
        }
        private void AddItems(Account p, ItemsModel item)
        {
            try
            {
                ItemsModel modelo = new ItemsModel(item) { _objId = item._objId };
                if (_type == 1)
                    PlayerManager.TryCreateItem(modelo, p._inventory, p.player_id);
                SEND_ITEM_INFO.LoadItem(p, modelo);
                switch(modelo._category)
                {
                    case 1: weapons.Add(modelo); break;
                    case 2: charas.Add(modelo); break;
                    case 3: cupons.Add(modelo); break;
                }
            }
            catch (Exception ex)
            {
                p.Close(0);
                SendDebug.SendInfo("[INVENTORY_ITEM_CREATE_PAK2] " + ex.ToString());
            }
        }
    }
}