using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.data.sync.client_side;
using Game.Progress;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_MISSION_BOMB_INSTALL_REC : ReceiveGamePacket
    {
        private int slotIdx;
        private float x, y, z;
        private byte area;
        public BATTLE_MISSION_BOMB_INSTALL_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotIdx = ReadD();
            area = ReadC();
            x = ReadT();
            y = ReadT();
            z = ReadT();
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                Room room = player?._room;
                if (room != null && room.round.Timer == null && room._state == RoomState.Battle && !room.C4_actived && room.room_type == 2)
                {
                    SLOT slot = room.GetSlot(slotIdx);
                    if (slot == null || slot.state != SLOT_STATE.BATTLE || slot._team != 0)
                        return;
                      Net_Room_C4.InstallBomb(room, slot, area, x, y, z);
                }
            }
            catch (Exception ex)
            {
                Logger.Info("[BATTLE_MISSION_BOMB_INSTALL_REC]: " + ex.ToString());
            }
        }
    }
}