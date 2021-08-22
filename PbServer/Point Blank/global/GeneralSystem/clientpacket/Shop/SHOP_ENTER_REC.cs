using Core;
using Core.managers;
using Core.models.account.players;
using Core.models.enums;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class SHOP_ENTER_REC : ReceiveGamePacket
    {
        public SHOP_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
           ReadD();
        }

        public override void Run()
        {
            try
            {
                if (_client == null)
                    return;
                Account p = _client._player;
                Room room = p?._room;
                if (room != null)
                {
                    room.ChangeSlotState(p._slotId, SLOT_STATE.SHOP, false);
                    room.StopCountDown(p._slotId);
                    room.UpdateSlotsInfo();
                }
                _client.SendPacket(new SHOP_ENTER_PAK());
                if (Settings.BlackFriday && !p.fistShoplobby)
                {
                    p.fistShoplobby = true;
                    _client.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK("[System] " + LorenstudioSettings.ProjectName + " Sale event, Shop for half price."));
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}