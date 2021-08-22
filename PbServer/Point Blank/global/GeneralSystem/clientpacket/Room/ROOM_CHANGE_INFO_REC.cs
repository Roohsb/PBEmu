using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_CHANGE_INFO_REC : ReceiveGamePacket
    {
        public ROOM_CHANGE_INFO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }
        public override void Read()
        {
            try
            {
                Account player = _client._player;
                Room room = player?._room;
                if (room != null && room._leader == player._slotId && room._state == RoomState.Ready)
                {
                    ReadD();
                    room.name = ReadS(23);
                    room.mapId = ReadH();
                    room.stage4v4 = ReadC();
                    byte stageType = ReadC();
                    if (stageType != room.room_type)
                    {
                        room.room_type = stageType;
                        int count = 0;
                        for (int i = 0; i < 16; i++)
                        {
                            SLOT slot = room._slots[i];
                            if ((int)slot.state == 8)
                            {
                                slot.state = SLOT_STATE.NORMAL;
                                count++;
                            }
                        }
                        if (count > 0)
                            room.UpdateSlotsInfo();
                    }
                    ReadC();
                    ReadC();
                    ReadC();
                    room._ping = ReadC();
                    room.weaponsFlag = ReadC();
                    room.random_map = ReadC();
                    room.special = ReadC();
                    ReadS(33);
                    room.killtime = ReadC();
                    ReadC();
                    ReadC();
                    ReadC();
                    room.limit = ReadC();
                    room.seeConf = ReadC();
                    room.autobalans = ReadH();
                    room.aiCount = ReadC();
                    room.aiLevel = ReadC();
                    room.UpdateRoomInfo();
                }
            }
            catch (Exception ex)
            {
                Logger.Info("ROOM_CHANGE_INFO_REC: " + ex.ToString());
            }
        }

        public override void Run()
        {
        }
    }
}