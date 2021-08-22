﻿using Core.server;

namespace Game.global.serverpacket
{
    public class FRIEND_REMOVE_PAK : SendPacket
    {
        private uint _erro;
        public FRIEND_REMOVE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(285);
            WriteD(_erro);
        }
    }
}