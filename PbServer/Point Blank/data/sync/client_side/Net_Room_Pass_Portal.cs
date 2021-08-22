using Core;
using Core.models.enums;
using Core.models.enums.missions;
using Core.models.room;
using Core.server;
using Game.data.model;
using Game.data.utils;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.data.sync.client_side
{
    public static class Net_Room_Pass_Portal
    {
        public static void Load(ReceiveGPacket p)
        {
            int roomId = p.ReadH();
            int channelId = p.ReadH();
            int slotId = p.ReadC(); //player
            int portalId = p.ReadC(); //portal
            Channel ch = ChannelsXML.getChannel(channelId);
            if (ch == null)
                return;
            Room room = ch.GetRoom(roomId);
            if (room != null && room.round.Timer == null && room._state == RoomState.Battle && room.room_type == 7)
            {
                SLOT slot = room.GetSlot(slotId);
                if (slot != null && slot.state == SLOT_STATE.BATTLE)
                {
                    ++slot.passSequence;
                    if (slot._team == 0) room.red_dino += 5;
                    else room.blue_dino += 5;
                    CompleteMission(room, slot);
                    using BATTLE_MISSION_ESCAPE_PAK packet = new BATTLE_MISSION_ESCAPE_PAK(room, slot);
                    using BATTLE_DINO_PLACAR_PAK packet2 = new BATTLE_DINO_PLACAR_PAK(room);
                    room.SendPacketToPlayers(packet, packet2, SLOT_STATE.BATTLE, 0);
                }
            }
            if (p.GetBuffer().Length > 8)
                SendDebug.SendInfo("[Invalid PORTAL: " + BitConverter.ToString(p.GetBuffer()) + "]");
        }
        private static void CompleteMission(Room room, SLOT slot)
        {
            MISSION_TYPE mission;
            switch (slot.passSequence)
            {
                case 1: mission = MISSION_TYPE.TOUCHDOWN;
                    AllUtils.CompleteMission(room, slot, mission, 0); break;
                case 2: mission = MISSION_TYPE.TOUCHDOWN_ACE_ATTACKER; 
                    AllUtils.CompleteMission(room, slot, mission, 0); break;
                case 3: mission = MISSION_TYPE.TOUCHDOWN_HATTRICK; 
                    AllUtils.CompleteMission(room, slot, mission, 0); break;
                case 4: mission = MISSION_TYPE.TOUCHDOWN_GAME_MAKER; 
                    AllUtils.CompleteMission(room, slot, mission, 0);  break;
            } 
        }
    }
}