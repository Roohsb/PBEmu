using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.shop;
using Core.server;
using Game.data.model;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Game.global.serverpacket
{
    public class SHOP_BUY_PAK : SendPacket
    {
        private List<ItemsModel> weapons = new List<ItemsModel>(),
            charas = new List<ItemsModel>(),
            cupons = new List<ItemsModel>();
        private Account p;
        private uint erro;
        public SHOP_BUY_PAK(uint erro, List<GoodItem> item = null, Account player = null)
        {
            this.erro = erro;
            if (this.erro == 1)
            {
                p = player;
                AddItems(item);
            }
        }
        public override void Write()
        {
            WriteH(531);
            WriteD(erro);
            if (erro == 1)
            {
                WriteD(uint.Parse(DateTime.Now.ToString("yyMMddHHmm")));
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
                WriteD(p._gp);
                WriteD(p._money);
            }
        }
        private void AddItems(List<GoodItem> items)
        {
            GoodItem g2 = null;
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    GoodItem good = items[i];
                    g2 = good;
                    ItemsModel iv = p._inventory.getItem(good._item._id);

                    ItemsModel modelo = new ItemsModel(good._item);
                    if (iv == null)
                    {
                        if (PlayerManager.CreateItem(modelo, p.player_id))
                            p._inventory.AddItem(modelo);
                    }
                    else
                    {
                        modelo._count = iv._count;
                        modelo._objId = iv._objId;
                        if (iv._equip == 1)
                        {
                            modelo._count += good._item._count;
                            ComDiv.UpdateDB("player_items", "count", (long)modelo._count, "owner_id", p.player_id, "item_id", modelo._id);
                        }
                        else if (iv._equip == 2 && modelo._category != 3)
                        {
                            DateTime data = DateTime.ParseExact(iv._count.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);
                            modelo._count = uint.Parse(data.AddSeconds(good._item._count).ToString("yyMMddHHmm"));
                            ComDiv.UpdateDB("player_items", "count", (long)modelo._count, "owner_id", p.player_id, "item_id", modelo._id);
                        }
                        modelo._equip = iv._equip;
                        iv._count = modelo._count;
                    }
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
                erro = 2147487767;
                SendDebug.SendInfo("[SHOP_BUY_PAK] " + ex.ToString());
                if (g2 != null)
                    SendDebug.SendInfo("[SHOP_BUY_PAK] Good: " + g2.id);
            }
        }
    }
}