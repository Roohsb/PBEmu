using Core;
using Core.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class RefillShop
    {
        public static string InstantRefill(Account player1, bool friday)
        {
            ShopManager.Reset();
            ShopManager.Load(1, friday);
            foreach (System.Collections.Generic.KeyValuePair<uint, GameClient> client in GameManager._socketList)
            {
                GameClient game = client.Value;
                if (game != null && game._player != null && game.player_id > 0)
                {
                    Account player = game._player;
                    for (int i = 0; i < ShopManager.ShopDataItems.Count; i++)
                        player.SendPacket(new SHOP_GET_ITEMS_PAK(ShopManager.ShopDataItems[i], ShopManager.TotalItems));
                    for (int i = 0; i < ShopManager.ShopDataGoods.Count; i++)
                        player.SendPacket(new SHOP_GET_GOODS_PAK(ShopManager.ShopDataGoods[i], ShopManager.TotalGoods));
                    player.SendPacket(new SHOP_GET_REPAIR_PAK());
                    player.SendPacket(new SHOP_TEST2_PAK());
                    int cafe = (int)player.pc_cafe;
                    switch (cafe)
                    {
                        case 0:
                            for (int i = 0; i < ShopManager.ShopDataMt1.Count; i++)
                                player.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMt1[i], ShopManager.TotalMatching1));
                            break;
                        case 1:
                        case 2:
                            for (int i = 0; i < ShopManager.ShopDataMt2.Count; i++)
                                player.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMt2[i], ShopManager.TotalMatching2));
                            break;
                        case 3: break;
                        case 4: break;
                        case 5:
                        case 6:
                            for (int i = 0; i < ShopManager.ShopDataMt3.Count; i++)
                                player.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMt3[i], ShopManager.TotalMatching3));
                            break;
                    }
                    player.SendPacket(new SHOP_LIST_PAK());
                }
            }
            SendDebug.SendInfo(Translation.GetLabel("RefillShopWarn", player1.player_name));
            return Translation.GetLabel("RefillShopMsg");
        }
    }
}