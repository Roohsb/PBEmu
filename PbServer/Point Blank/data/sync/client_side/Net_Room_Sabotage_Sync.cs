﻿using Core;
using Core.models.enums;
using Core.models.room;
using Core.server;
using Game.data.model;
using Game.data.utils;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.data.sync.client_side
{
    public class Net_Room_Sabotage_Sync
    {
        public static void Load(ReceiveGPacket p)
        {
            int roomId = p.ReadH();
            int channelId = p.ReadH();
            byte killerIdx = p.ReadC();
            ushort redObjective = p.ReadUH();
            ushort blueObjective = p.ReadUH();
            int barNumber = p.ReadC();
            ushort damage = p.ReadUH();
            if (p.GetBuffer().Length > 14)
                SendDebug.SendInfo("[Invalid SABOTAGE: " + BitConverter.ToString(p.GetBuffer()) + "]");
            Channel ch = ChannelsXML.getChannel(channelId);
            if (ch == null)
                return;
            Room room = ch.GetRoom(roomId);
            SLOT killer;
            if (room != null && room.round.Timer == null && room._state == RoomState.Battle && !room.swapRound && room.GetSlot(killerIdx, out killer))
            {
                room.Bar1 = redObjective;
                room.Bar2 = blueObjective;
                RoomType type = (RoomType)room.room_type;
                int times = 0;
                if (barNumber == 1)
                {
                    killer.damageBar1 += damage;
                    times += killer.damageBar1 / 600;
                }
                else if (barNumber == 2)
                {
                    killer.damageBar2 += damage;
                    times += killer.damageBar2 / 600;
                }
                killer.earnedXP = times;
                if (type == RoomType.Destroy)
                {
                    using (BATTLE_MISSION_GENERATOR_INFO_PAK packet = new BATTLE_MISSION_GENERATOR_INFO_PAK(room))
                        room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                    if (room.Bar1 == 0) EndRound(room, 1);
                    else if (room.Bar2 == 0) EndRound(room, 0);
                }
                else if (type == RoomType.Defense)
                {
                    using (BATTLE_MISSION_DEFENCE_INFO_PAK packet = new BATTLE_MISSION_DEFENCE_INFO_PAK(room))
                        room.SendPacketToPlayers(packet, SLOT_STATE.BATTLE, 0);
                    if (room.Bar1 == 0 && room.Bar2 == 0)
                        EndRound(room, 0);
                }
            }
        }
        public static void EndRound(Room room, byte winner)
        {
            room.swapRound = true;
            if (winner == 0) room.red_rounds++;
            else if (winner == 1) room.blue_rounds++;
            AllUtils.BattleEndRound(room, winner, RoundEndType.Normal);
        }
    }
}