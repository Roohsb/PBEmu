﻿using Core.server;

namespace Game.global.serverpacket
{
    public class CLAN_PRIVILEGES_MASTER_PAK : SendPacket
    {
        public CLAN_PRIVILEGES_MASTER_PAK()
        {
        }

        public override void Write()
        {
            WriteH(1339);
        }
    }
}