using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_CLOSE_SLOT_REC : ReceiveGamePacket
    {
        private int slotInfo;
        private uint erro;
        public ROOM_CLOSE_SLOT_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotInfo = ReadD(); //0x10000000 = Abrindo slot fechado || 0 = Fechando slot
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                Room room = p?._room;
                if (room != null && room._leader == p._slotId)
                {
                    SLOT slot = room.GetSlot(slotInfo & 0xFFFFFFF);
                    if (slot == null)
                        return;
                    if ((slotInfo & 0x10000000) == 0x10000000)
                        OpenSlot(room, slot);
                    else
                        CloseSlot(room, slot);
                }
                else erro = 2147484673;
                _client.SendPacket(new ROOM_CLOSE_SLOT_PAK(erro));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[ROOM_CLOSE_SLOT_REC] " + ex.ToString());
            }
        }
        private void CloseSlot(Room room, SLOT slot)
        {
            switch (slot.state)
            {
                case SLOT_STATE.EMPTY:room.ChangeSlotState(slot, SLOT_STATE.CLOSE, true);break;
                case SLOT_STATE sTATE when (sTATE == SLOT_STATE.CLAN || sTATE == SLOT_STATE.NORMAL || sTATE == SLOT_STATE.INFO ||
                   sTATE == SLOT_STATE.INVENTORY || sTATE == SLOT_STATE.OUTPOST || sTATE == SLOT_STATE.SHOP || sTATE == SLOT_STATE.READY):
                    Account player = room.GetPlayerBySlot(slot);
                    if (player != null && !player.AntiKickGM)
                    {
                        if (((int)slot.state != 8 && (room._channelType == 4 && (int)room._state != 1 || room._channelType != 4) ||
                            (int)slot.state == 8 && (room._channelType == 4 && (int)room._state == 0 || room._channelType != 4)))
                        {
                            player.SendPacket(new SERVER_MESSAGE_KICK_PLAYER_PAK()); //2147484673 - 4vs4 error
                            room.RemovePlayer(player, slot, false);
                        }
                    }
                    break;
            }
        }
        private void OpenSlot(Room room, SLOT slot)
        {
            if (((slotInfo & 0x10000000) != 0x10000000) || slot.state != SLOT_STATE.CLOSE)
                return;
            room.ChangeSlotState(slot, SLOT_STATE.EMPTY, true);
        }
    }
}