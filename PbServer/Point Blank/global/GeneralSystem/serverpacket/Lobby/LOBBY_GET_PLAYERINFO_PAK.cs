using Core.models.account.players;
using Core.server;

namespace Game.global.serverpacket
{
    public class LOBBY_GET_PLAYERINFO_PAK : SendPacket
    {
        private PlayerStats st;
        public LOBBY_GET_PLAYERINFO_PAK(PlayerStats stats)
        {
            st = stats;
        }

        public override void Write()
        {
            WriteH(2640);
            if (st != null)
            {
                WriteD(st.fights);
                WriteD(st.fights_win);
                WriteD(st.fights_lost);
                WriteD(st.fights_draw);
                WriteD(st.kills_count);
                WriteD(st.headshots_count);
                WriteD(st.deaths_count);
                WriteD(st.totalfights_count);
                WriteD(st.totalkills_count);
                WriteD(st.escapes);
                WriteD(st.fights);
                WriteD(st.fights_win);
                WriteD(st.fights_lost);
                WriteD(st.fights_draw);
                WriteD(st.kills_count);
                WriteD(st.headshots_count);
                WriteD(st.deaths_count);
                WriteD(st.totalfights_count);
                WriteD(st.totalkills_count);
                WriteD(st.escapes);
            }
            else
                WriteB(new byte[80]);
        }
    }
}