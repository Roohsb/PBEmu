﻿using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_USER_EXIT_PAK : SendPacket
    {
        public BASE_USER_EXIT_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2655);
        }
    }
}