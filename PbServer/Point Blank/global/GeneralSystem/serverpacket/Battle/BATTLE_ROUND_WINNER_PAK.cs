﻿using Core.models.enums;
using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BATTLE_ROUND_WINNER_PAK : SendPacket
    {
        private Room _room;
        private int _winner;
        private RoundEndType _reason;
        public BATTLE_ROUND_WINNER_PAK(Room room, int winner, RoundEndType reason)
        {
            _room = room;
            _winner = winner;
            _reason = reason;
        }
        public BATTLE_ROUND_WINNER_PAK(Room room, TeamResultType winner, RoundEndType reason)
        {
            _room = room;
            _winner = (int)winner;
            _reason = reason;
        }
        public override void Write()
        {
            WriteH(3353);
            WriteC((byte)_winner); //empate = 2
            WriteC((byte)_reason); //empate = 1
            if (_room.room_type == 7)
            {
                WriteH((ushort)_room.red_dino);
                WriteH((ushort)_room.blue_dino);
            }
            else if (_room.room_type == 1 || _room.room_type == 8 || _room.room_type == 13)
            {
                WriteH((ushort)_room._redKills);
                WriteH((ushort)_room._blueKills);
            }
            else
            {
                WriteH((ushort)_room.red_rounds);
                WriteH((ushort)_room.blue_rounds);
            }
        }
    }
}