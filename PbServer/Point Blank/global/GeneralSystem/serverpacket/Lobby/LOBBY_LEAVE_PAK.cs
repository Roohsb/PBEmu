﻿using Core.server;

namespace Game.global.serverpacket
{
    public class LOBBY_LEAVE_PAK : SendPacket
    {
        private uint _erro;
        public LOBBY_LEAVE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(3084);
            WriteD(_erro);
        }
    }
}