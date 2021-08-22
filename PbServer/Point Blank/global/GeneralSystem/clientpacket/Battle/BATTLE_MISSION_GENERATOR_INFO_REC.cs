using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.model;
using Game.data.sync.client_side;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_MISSION_GENERATOR_INFO_REC : ReceiveGamePacket
    {
        private ushort barRed, barBlue;
        private List<ushort> damages = new List<ushort>();
        public BATTLE_MISSION_GENERATOR_INFO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }
        public override void Read()
        {
            barRed = ReadUH();
            barBlue = ReadUH();
            int i = 0;
            do
            {
                damages.Add(ReadUH());

            } while (++i < 16);
        }
        public override void Run()
        {
            try
            {
                Account player = _client._player;
                Room room = player?._room;
                if (room != null && room.round.Timer == null && room._state == RoomState.Battle && !room.swapRound && room.room_type == 3)
                {
                    SLOT slot = room.GetSlot(player._slotId);
                    if (slot == null || slot.state != SLOT_STATE.BATTLE || slot._team != 0)
                        return;
                    room.Bar1 = barRed;
                    room.Bar2 = barBlue;
                    int SlotID = 0;
                    do
                    {
                        SLOT slotR = room._slots[SlotID];
                        if (slotR._playerId > 0 && (int)slotR.state == 13)
                        {
                            slotR.damageBar1 = damages[SlotID];
                            slotR.damageBar2 = damages[SlotID];
                            slotR.earnedXP = damages[SlotID] / 600; //VALOR DE XP 
                        }
                    } while (++SlotID < 16);
                    using (BATTLE_MISSION_GENERATOR_INFO_PAK packet = new BATTLE_MISSION_GENERATOR_INFO_PAK(room))
                        room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                    if (barRed == 0) Net_Room_Sabotage_Sync.EndRound(room, 1);
                    else
                    if (barBlue == 0) Net_Room_Sabotage_Sync.EndRound(room, 0);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}