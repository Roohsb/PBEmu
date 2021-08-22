﻿using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_JOINED_ROOM_PAK : SendPacket
    {
        private Match _mt;
        private int _roomId, _team;
        public CLAN_WAR_JOINED_ROOM_PAK(Match match, int roomId, int team)
        {
            _mt = match;
            _roomId = roomId;
            _team = team;
        }

        public override void Write()
        {
            WriteH(1566);
            WriteD(_roomId);
            WriteH((ushort)_team);
            WriteH((ushort)_mt.GetServerInfo());
        }
    }
}