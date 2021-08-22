﻿using Core.server;

namespace Game.global.serverpacket
{
    public class BOX_MESSAGE_CREATE_PAK : SendPacket
    {
        private uint _erro;
        public BOX_MESSAGE_CREATE_PAK(uint erro)
        {
            _erro = erro;
        }

        public override void Write()
        {
            WriteH(418);
            WriteD(_erro);
        }
    }
}