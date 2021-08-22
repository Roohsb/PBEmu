using Core;
using Core.models.enums;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_PROFILE_LEAVE_REC : ReceiveGamePacket
    {
        public BASE_PROFILE_LEAVE_REC(GameClient client, byte[] data)
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
                Account p = _client._player;
                if (p == null)
                    return;
                Room room = p._room;
                if (room != null)
                    room.ChangeSlotState(p._slotId, SLOT_STATE.NORMAL, true);
                _client.SendPacket(new BASE_PROFILE_LEAVE_PAK());
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}