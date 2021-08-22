using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_CHANGE_DIFFICULTY_LEVEL_REC : ReceiveGamePacket
    {
        public BATTLE_CHANGE_DIFFICULTY_LEVEL_REC(GameClient client, byte[] data)
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
                Room room = p?._room;
                if (room == null || room._state != RoomState.Battle || room.IngameAiLevel >= 10)
                    return;
                SLOT slot = room.GetSlot(p._slotId);
                if (slot == null || slot.state != SLOT_STATE.BATTLE)
                    return;
                if (room.IngameAiLevel <= 9)
                    room.IngameAiLevel++;
                using BATTLE_CHANGE_DIFFICULTY_LEVEL_PAK packet = new BATTLE_CHANGE_DIFFICULTY_LEVEL_PAK(room);
                room.SendPacketToPlayers(packet, SLOT_STATE.READY, 1);
            }
            catch (Exception ex)
            {
                Logger.Info("BATTLE_CHANGE_DIFFICULTY_LEVEL_REC: " + ex.ToString());
            }
        }
    }
}