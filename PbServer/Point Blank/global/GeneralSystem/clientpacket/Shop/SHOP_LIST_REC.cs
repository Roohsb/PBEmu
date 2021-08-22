using Core;
using Core.managers;
using Core.models.account.players;
using Core.xml;
using Game.data.chat;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class SHOP_LIST_REC : ReceiveGamePacket
    {
        private int erro;
        private Account p;
        public SHOP_LIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }
        public override void Read()
        {
            erro = ReadD();
        }
        public override void Run()
        {
            try
            {
                p = _client._player;
                if (p == null)
                    return;
                if (p.LoadedShop || erro > 0)
                    goto SendPacket;
                for (int i = 0; i < ShopManager.ShopDataItems.Count; i++)
                    _client.SendPacket(new SHOP_GET_ITEMS_PAK(ShopManager.ShopDataItems[i], ShopManager.TotalItems));
                for (int i = 0; i < ShopManager.ShopDataGoods.Count; i++)
                    _client.SendPacket(new SHOP_GET_GOODS_PAK(ShopManager.ShopDataGoods[i], ShopManager.TotalGoods));
                _client.SendPacket(new SHOP_GET_REPAIR_PAK());
                _client.SendPacket(new SHOP_TEST2_PAK());
                int cafe = (int)p.pc_cafe;
                switch (cafe)
                {
                    case 0:
                        for (int i = 0; i < ShopManager.ShopDataMt1.Count; i++)
                            _client.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMt1[i], ShopManager.TotalMatching1));
                        break;
                    case 1:
                    case 2:
                        for (int i = 0; i < ShopManager.ShopDataMt2.Count; i++)
                            _client.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMt2[i], ShopManager.TotalMatching2));
                        break;
                    case 3: break;
                    case 4: break;
                    case 5:
                    case 6:
                        for (int i = 0; i < ShopManager.ShopDataMt3.Count; i++)
                            _client.SendPacket(new SHOP_GET_MATCHING_PAK(ShopManager.ShopDataMt3[i], ShopManager.TotalMatching3));
                        break;
                }
                try
                {
                    BemVindo();
                }
                catch
                {
                    using LOBBY_CHATTING_PAK txt1 = new LOBBY_CHATTING_PAK("Error", p.GetSessionId(), p.name_color, true, "found contact the ADMS.");
                    p.SendPacket(txt1);
                }

                SendPacket:
                _client.SendPacket(new SHOP_LIST_PAK());

            }
            catch (Exception ex)
            {
                Logger.Info("SHOP_LIST_REC: " + ex.ToString());
            }
        }
        public void BemVindo()
        {
            if (!p.LoadedShop)
            {
                string strOns = "- Online Players [" + GameManager.PlayersOnlines() + " / " + ServersXML.getServer(1)._maxPlayers + "] ";
                // string conta = p.player_name.Length == 0 ? "- New Account? Welcome to '" + LorenstudioSettings.ProjectName + "'" : "- Hi '" + p.player_name + "' good to see you again, get your vip and have unique advantages!";
                // string str1 = "-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-";
                // string str2 = conta;
                //string str3 = "- Common players have commands, type in chat (     !ajuda     ) and see!!! ";
                //string str6 = "-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx-";

                //using LOBBY_CHATTING_PAK txt1 = new LOBBY_CHATTING_PAK("", p.GetSessionId(), p.name_color, true, str1);
                //  p.SendPacket(txt1);
                // using LOBBY_CHATTING_PAK txt2 = new LOBBY_CHATTING_PAK(Settings.BOT_Name, p.GetSessionId(), p.name_color, true, str2);
                // p.SendPacket(txt2);
                //  using LOBBY_CHATTING_PAK txt3 = new LOBBY_CHATTING_PAK(Settings.BOT_Name, p.GetSessionId(), p.name_color, true, str3);
                //  p.SendPacket(txt3);
                //using LOBBY_CHATTING_PAK txt4 = new LOBBY_CHATTING_PAK(Settings.BOT_Name, p.GetSessionId(), p.name_color, true, str4);
                //p.SendPacket(txt4);
                // using LOBBY_CHATTING_PAK txt5 = new LOBBY_CHATTING_PAK(Settings.BOT_Name, p.GetSessionId(), p.name_color, true, str5);
                //p.SendPacket(txt5);
                //  using LOBBY_CHATTING_PAK txt6 = new LOBBY_CHATTING_PAK("", p.GetSessionId(), p.name_color, true, str6);
                //  p.SendPacket(txt6);

                p.LoadedShop = true;
            }
        }
    }
}