using Core;
using Core.managers;
using Core.models.shop;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class SHOP_BUY_ITEM_REC : ReceiveGamePacket
    {
        public int count = 0;
        private List<CartGoods> ShopCart = new List<CartGoods>();
        public SHOP_BUY_ITEM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }
        public override void Read()
        {
            count = ReadC();
            for (int i = 0; i < count; i++)
            {
                ShopCart.Add(new CartGoods
                {
                    GoodId = ReadD(),
                    BuyType = ReadC()
                });
            }
        }
        public override void Run()
        {
            try
            {
                Account p = _client._player;
                List<GoodItem> items = ShopManager.GetGoods(ShopCart, out int gold, out int cash);
                if(CheckBuy(p, items, gold, cash))
                {
                    if (PlayerManager.UpdateAccountCashing(p.player_id, (p._gp - gold), (p._money - cash)))
                    {
                        p._gp -= gold;
                        p._money -= cash;
                        _client.SendPacket(new 	SHOP_BUY_PAK(1, items, p));
                    }
                    else
                        _client.SendPacket(new SHOP_BUY_PAK(2147487769));
                }

            }
            catch (Exception ex)
            {
                Logger.Info("SHOP_BUY_ITEM_REC: " + ex.ToString());
            }
        }
        public bool CheckBuy(Account p, List<GoodItem> items, int gold, int cash)
        {
            try
            {
                if(p == null || p.player_name.Length == 0)
                {
                    _client.SendPacket(new SHOP_BUY_PAK(2147487767));
                    return false;
                }
                else if(p._inventory._items.Count >= 500)
                {
                    _client.SendPacket(new SHOP_BUY_PAK(2147487929));
                    return false;
                }
                else if (items.Count == 0)
                {
                    _client.SendPacket(new SHOP_BUY_PAK(2147487767));
                    return false;
                }
                else if (0 > (p._gp - gold) || 0 > (p._money - cash))
                {
                    _client.SendPacket(new SHOP_BUY_PAK(2147487768));
                    return false;
                }
                else
                    return true;
            }
            catch(Exception ex)
            {
                Logger.Info("SHOP_BUY_ITEM_REC 2: " + ex.ToString());
                return false;
            }
        }
    }
}