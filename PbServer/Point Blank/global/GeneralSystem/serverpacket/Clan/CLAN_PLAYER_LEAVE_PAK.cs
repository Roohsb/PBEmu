﻿using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_PLAYER_LEAVE_PAK : SendPacket
    {
        private uint _erro;
        public CLAN_PLAYER_LEAVE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(1333);
            WriteD(_erro);
        }
    }
}