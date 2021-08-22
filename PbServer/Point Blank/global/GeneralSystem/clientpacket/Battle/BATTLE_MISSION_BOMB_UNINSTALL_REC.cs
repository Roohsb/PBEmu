using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.data.sync.client_side;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_MISSION_BOMB_UNINSTALL_REC : ReceiveGamePacket
    {
        private int slotIdx;
        public BATTLE_MISSION_BOMB_UNINSTALL_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotIdx = ReadD();
        }

        public override void Run()
        {
            Account player = _client._player;
            Room room = player?._room;
            if (room != null && room.round.Timer == null && room._state == RoomState.Battle && room.C4_actived && room.room_type == 2)
            {
                SLOT slot = room.GetSlot(slotIdx);
                if (slot == null || slot.state != SLOT_STATE.BATTLE || slot._team != 1)
                    return;
                Net_Room_C4.UninstallBomb(room, slot);
            }
        }
    }
}