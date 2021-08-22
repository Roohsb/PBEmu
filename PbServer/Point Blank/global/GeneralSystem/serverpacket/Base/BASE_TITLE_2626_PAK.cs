using Core.server;
using Game.data.model;
using System;

namespace Game.global.serverpacket
{
    public class BASE_2626_PAK : SendPacket
    {
        private Account p;
        public BASE_2626_PAK(Account player)
        {
            p = player;
        }

        public override void Write()
        {
            WriteH(2626);
            WriteB(BitConverter.GetBytes(p.player_id), 0, 4);
            WriteQ(p._titles.Flags);
            WriteC((byte)p._titles.Equiped1);
            WriteC((byte)p._titles.Equiped2);
            WriteC((byte)p._titles.Equiped3);
            WriteD(p._titles.Slots);
        }
    }
}