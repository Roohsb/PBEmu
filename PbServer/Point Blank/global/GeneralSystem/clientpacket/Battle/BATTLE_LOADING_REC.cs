using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_LOADING_REC : ReceiveGamePacket
    {
        private string name;
        public BATTLE_LOADING_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            name = ReadS(ReadC());
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Room room = p._room;
                if (room != null && room.IsPreparing() && room.GetSlot(p._slotId, out SLOT slot) && slot.state == SLOT_STATE.LOAD)
                {
                    slot.preLoadDate = DateTime.Now;
                    room.StartCounter(0, p, slot);
                    room.ChangeSlotState(slot, SLOT_STATE.RENDEZVOUS, true);
                    room._mapName = name;
                    if (slot._id == room._leader)
                    {
                        AllUtils.LogRoomBattleStart(room);
                        room._state = RoomState.Rendezvous;
                        room.UpdateRoomInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Info("BATTLE_LOADING_REC: " + ex.ToString());
            }
        }
    }
}