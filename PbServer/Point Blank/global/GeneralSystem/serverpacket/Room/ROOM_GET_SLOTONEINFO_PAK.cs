using Core.models.account.clan;
using Core.models.enums.flags;
using Core.server;
using Game.data.managers;
using Game.data.model;
using System;

namespace Game.global.serverpacket
{
    public class ROOM_GET_SLOTONEINFO_PAK : SendPacket
    {
        private Account p;
        private Clan clan;
        public ROOM_GET_SLOTONEINFO_PAK(Account player)
        {
            p = player;
            if (p != null)
                clan = ClanManager.GetClan(p.clanId);
        }
        public ROOM_GET_SLOTONEINFO_PAK(Account player, Clan c)
        {
            p = player;
            clan = c;
        }
        public override void Write()
        {
            WriteH(3909);
            WriteD(p._slotId);
            WriteC((byte)p._room._slots[p._slotId].state);
            WriteC((byte)p.GetRank());
            WriteD(clan._id);
            WriteD(p.clanAccess);
            WriteC((byte)clan._rank);
            WriteD(clan._logo);
            WriteC((byte)p.pc_cafe >= 5 ? (byte)2 : (byte)p.pc_cafe);
            WriteC((byte)p.tourneyLevel);
            WriteD((uint)p.effects);
            WriteS(clan._name, 17);
            WriteD(0);
            WriteC(31);
            WriteS(p.player_name, 33);
            WriteC((byte)p.name_color);
        }
    }
}