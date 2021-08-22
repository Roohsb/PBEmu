﻿using Core.server;

namespace Game.global.serverpacket
{
    public class AUTH_RANDOM_BOX_REWARD_PAK : SendPacket
    {
        private int _cupomId, _rnd;
        public AUTH_RANDOM_BOX_REWARD_PAK(int cupomId, int index)
        {
            _cupomId = cupomId;
            _rnd = index;
        }

        public override void Write()
        {
            WriteH(551);
            WriteD(_cupomId);
            WriteC((byte)_rnd);
        }
    }
}