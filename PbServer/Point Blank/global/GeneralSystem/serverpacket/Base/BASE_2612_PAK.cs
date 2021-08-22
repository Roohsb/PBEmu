using Core.models.account.clan;
using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class BASE_2612_PAK : SendPacket
    {
        private Account p;
        private Clan clan;
        public BASE_2612_PAK(Account player)
        {
            p = player;
            clan = ClanManager.GetClan(p.clanId);
        }

        public override void Write()
        {
            WriteH(2612);
            WriteS(p.player_name, 33);
            WriteD(p._exp);
            WriteD(p._rank);
            WriteD(p._rank);
            WriteD(p._gp);
            WriteD(p._money);
            WriteD(clan._id);
            WriteD(p.clanAccess);
            WriteQ(0);
            WriteC((byte)p.pc_cafe);
            WriteC((byte)p.tourneyLevel);
            WriteC((byte)p.name_color);
            WriteS(clan._name, 17);
            WriteC((byte)clan._rank);
            WriteC((byte)clan.GetClanUnit());
            WriteD(clan._logo);
            WriteC((byte)clan._name_color);
            WriteD(10000);
            WriteC(0);
            WriteD(0);
            WriteD(p.LastRankUpDate); //109 BYTES
        }
    }
}