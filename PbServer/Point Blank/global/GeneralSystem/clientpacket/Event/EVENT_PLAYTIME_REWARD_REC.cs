using Core;
using Core.managers;
using Core.managers.events;
using Core.models.account.players;
using Core.models.shop;
using Core.server;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class EVENT_PLAYTIME_REWARD_REC : ReceiveGamePacket
    {
        private int goodId;
        public EVENT_PLAYTIME_REWARD_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            goodId = ReadD();
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                {
                    return;
                }
                PlayerEvent pev = player._event;
                GoodItem good = ShopManager.GetGood(goodId);
                if (good == null || pev == null)
                {
                    return;
                }
                PlayTimeModel eventPt = EventPlayTimeSyncer.GetRunningEvent();
                if (eventPt != null)
                {
                    uint count = (uint)eventPt.GetRewardCount(goodId);
                    if (pev.LastPlaytimeFinish == 1 && count > 0 && ComDiv.UpdateDB("player_events", "last_playtime_finish", 2, "player_id", _client.player_id))
                    {
                        pev.LastPlaytimeFinish = 2;
                        _client.SendPacket(new INVENTORY_ITEM_CREATE_PAK(0, player, new ItemsModel(good._item._id, good._item._category, "Playtime reward", good._item._equip, count)));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("EVENT_PLAYTIME_REWARD_REC] " + ex.ToString());
            }
        }
    }
}