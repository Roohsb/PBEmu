using Core.models.account.players;
using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_USER_CHANGE_STATS_PAK : SendPacket
    {
        private PlayerStats s;
        public BASE_USER_CHANGE_STATS_PAK(PlayerStats s)
        {
            this.s = s;
        }

        public override void Write()
        {
            WriteH(2610);
            WriteD(s.fights);
            WriteD(s.fights_win);
            WriteD(s.fights_lost);
            WriteD(s.fights_draw);
            WriteD(s.kills_count);
            WriteD(s.headshots_count);
            WriteD(s.deaths_count);
            WriteD(s.totalfights_count);
            WriteD(s.totalkills_count);
            WriteD(s.escapes);
            WriteD(s.fights);
            WriteD(s.fights_win);
            WriteD(s.fights_lost);
            WriteD(s.fights_draw);
            WriteD(s.kills_count);
            WriteD(s.headshots_count);
            WriteD(s.deaths_count);
            WriteD(s.totalfights_count);
            WriteD(s.totalkills_count);
            WriteD(s.escapes);
        }
    }
}