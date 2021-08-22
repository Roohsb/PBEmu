using Core.models.account.clan;
using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class ROOM_GET_PLAYERINFO_PAK : SendPacket
    {
        private Account p;
        public ROOM_GET_PLAYERINFO_PAK(Account player)
        {
            p = player;
        }

        public override void Write()
        {
            WriteH(3842);
            if (p == null || p._slotId == -1)
            {
                WriteD(0x80000000);
                return;
            }
            Clan clan = ClanManager.GetClan(p.clanId);
            WriteD(p._slotId);
            WriteS(p.player_name, 33);
            WriteD(p._exp);
            WriteD(p.GetRank());
            WriteD(p._rank);
            WriteD(p._gp);
            WriteD(p._money);
            WriteD(clan._id);
            WriteD(p.clanAccess);
            WriteD(0);
            WriteD(0);
            WriteC((byte)p.pc_cafe);
            WriteC((byte)p.tourneyLevel);
            WriteC((byte)p.name_color);
            WriteS(clan._name, 17);
            WriteC((byte)clan._rank);
            WriteC((byte)clan.GetClanUnit());
            WriteD(clan._logo);
            WriteC((byte)clan._name_color);
            WriteC(0);
            WriteD(0);
            WriteD(0);
            WriteD(p.LastRankUpDate);
            WriteD(p._statistic.fights);
            WriteD(p._statistic.fights_win);
            WriteD(p._statistic.fights_lost);
            WriteD(p._statistic.fights_draw);
            WriteD(p._statistic.kills_count);
            WriteD(p._statistic.headshots_count);
            WriteD(p._statistic.deaths_count);
            WriteD(p._statistic.totalfights_count);
            WriteD(p._statistic.totalkills_count);
            WriteD(p._statistic.escapes);
            WriteD(p._statistic.fights);
            WriteD(p._statistic.fights_win);
            WriteD(p._statistic.fights_lost);
            WriteD(p._statistic.fights_draw);
            WriteD(p._statistic.kills_count);
            WriteD(p._statistic.headshots_count);
            WriteD(p._statistic.deaths_count);
            WriteD(p._statistic.totalfights_count);
            WriteD(p._statistic.totalkills_count);
            WriteD(p._statistic.escapes);
            WriteD(p._equip._red);
            WriteD(p._equip._blue);
            WriteD(p._equip._helmet);
            WriteD(p._equip._beret);
            WriteD(p._equip._dino);
            WriteD(p._equip._primary);
            WriteD(p._equip._secondary);
            WriteD(p._equip._melee);
            WriteD(p._equip._grenade);
            WriteD(p._equip._special);
            WriteD(p._titles.Equiped1);
            WriteD(p._titles.Equiped2);
            WriteD(p._titles.Equiped3);
        }
    }
}