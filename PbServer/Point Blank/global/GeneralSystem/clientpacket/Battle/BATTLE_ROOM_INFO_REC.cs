using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_ROOM_INFO_REC : ReceiveGamePacket
    {
        public BATTLE_ROOM_INFO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            try
            {
                Account player = _client._player;
                Room room = player?._room;
                if (room == null || room._state != RoomState.Ready || room._leader != player._slotId)
                    return;
                ReadD();
                room.name = ReadS(23);
                room.mapId = ReadH();
                room.stage4v4 = ReadC();
                room.room_type = ReadC();
                ReadB(3);
                room._ping = ReadC();
                byte weaponsFlag = ReadC();
                if (weaponsFlag != room.weaponsFlag)
                {
                    room.weaponsFlag = weaponsFlag;
                    for (int i = 0; i < 16; i++)
                    {
                        SLOT slot = room._slots[i];
                        if ((int)slot.state == 8)
                            slot.state = SLOT_STATE.NORMAL;
                    }
                }
                room.random_map = ReadC();
                room.special = ReadC();
                room.aiCount = ReadC();
                room.aiLevel = ReadC();
                room.UpdateRoomInfo();
            }
            catch (Exception ex)
            {
                Logger.Info("BATTLE_ROOM_INFO_REC: " + ex.ToString());
            }
        }

        public override void Run()
        {
        }
    }
}