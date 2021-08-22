using Core;
using Core.models.enums;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_CHANGE_HOST_REC : ReceiveGamePacket
    {
        private int slotId;
        public ROOM_CHANGE_HOST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotId = ReadD();
        }

        public override void Run()
        {
            Account player = _client._player;
            Room room = player?._room;
            try
            {
                if (room == null || room._leader == slotId || room._slots[slotId]._playerId == 0)
                    _client.SendPacket(new ROOM_CHANGE_HOST_PAK(0x80000000));
                else if (room._state == RoomState.Ready && room._leader == player._slotId)
                {
                    room.SetNewLeader(slotId, 0, room._leader, false);
                    using (ROOM_CHANGE_HOST_PAK packet = new ROOM_CHANGE_HOST_PAK(slotId))
                        room.SendPacketToPlayers(packet);
                    room.UpdateSlotsInfo();
                }
            }
            catch (Exception ex)
            {
                Logger.Info("ROOM_CHANGE_HOST_REC: " + ex.ToString());
            }
        }
    }
}