using Core.models.account.clan;
using Core.models.enums;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.data.utils;

namespace Game.global.serverpacket
{
    public class BATTLE_ENDBATTLE_PAK : SendPacket
    {
        private Room r;
        private Account p;
        private TeamResultType winner = TeamResultType.TeamDraw;
        private ushort playersFlag, missionsFlag;
        private bool isBotMode;
        private byte[] array1;
        public BATTLE_ENDBATTLE_PAK(Account p)
        {
            this.p = p;
            if (p != null)
            {
                r = p._room;
                winner = AllUtils.GetWinnerTeam(r);
                isBotMode = r.IsBotMode();
                AllUtils.GetBattleResult(r, out missionsFlag, out playersFlag, out array1);
            }
        }
        public BATTLE_ENDBATTLE_PAK(Account p, TeamResultType winner, ushort playersFlag, ushort missionsFlag, bool isBotMode, byte[] a1)
        {
            this.p = p;
            this.winner = winner;
            this.playersFlag = playersFlag;
            this.missionsFlag = missionsFlag;
            this.isBotMode = isBotMode;
            array1 = a1;
            if (p != null)
                r = p._room;
        }
        public override void Write()
        {
            if (p == null || r == null)
                return;
            WriteH(3336);
            WriteC((byte)winner);
            WriteH(playersFlag);
            WriteH(missionsFlag);
            WriteB(array1);
            Clan clan = ClanManager.GetClan(p.clanId);
            WriteS(p.player_name, 33);
            WriteD(p._exp);
            WriteD(p._rank);
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
            WriteD(0);
            WriteC(0);
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
            if (isBotMode)
            {
                for (int i = 0; i < 16; i++)
                    WriteH((ushort)r._slots[i].Score);
            }
            else if (r.room_type == 2 || r.room_type == 4 || r.room_type == 7 || r.room_type == 12) //3 sabo || 4 defe
            {
                WriteH((ushort)(r.room_type == 7 ? r.red_dino : (r.room_type == 12 ? r._redKills : r.red_rounds)));
                WriteH((ushort)(r.room_type == 7 ? r.blue_dino : (r.room_type == 12 ? r._blueKills : r.blue_rounds)));
                for (int i = 0; i < 16; i++)
                    WriteC((byte)r._slots[i].objetivos);
            }
            WriteC(0);
            WriteD(0);
            WriteB(new byte[16]);
        }
    }
}