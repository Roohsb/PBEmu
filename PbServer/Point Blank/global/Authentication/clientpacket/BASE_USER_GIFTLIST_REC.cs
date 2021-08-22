using Core.managers;
using Core.models.account;
using Game.data.model;
using System;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_GIFTLIST_REC : ReceiveGamePacket
    {
        public BASE_USER_GIFTLIST_REC(GameClient lc, byte[] buff)
        {
            Inicial(lc, buff);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null || !LoginManager.Config.GiftSystem)
                    return;
                List<Message> gifts = MessageManager.GetGifts(player.player_id);
                if (gifts.Count > 0)
                {
                    MessageManager.RecicleMessages(player.player_id, gifts);
                    _client.SendPacket(new BASE_USER_GIFT_LIST_PAK(0, gifts));
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
    }
}