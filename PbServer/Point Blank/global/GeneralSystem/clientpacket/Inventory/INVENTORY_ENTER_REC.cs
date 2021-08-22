using Core;
using Core.models.enums;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class INVENTORY_ENTER_REC : ReceiveGamePacket
    {
        public INVENTORY_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
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
                    room.ChangeSlotState(p._slotId, SLOT_STATE.INVENTORY, false);
                    room.StopCountDown(p._slotId);
                    room.UpdateSlotsInfo();
                }
                _client.SendPacket(new INVENTORY_ENTER_PAK());
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
    }
}