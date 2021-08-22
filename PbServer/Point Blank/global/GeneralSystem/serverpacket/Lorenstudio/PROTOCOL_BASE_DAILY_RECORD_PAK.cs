using Core.server;
using Game.data.model;

namespace Game.global.GeneralSystem.serverpacket.Lorenstudio
{
    public class PROTOCOL_BASE_DAILY_RECORD_PAK : SendPacket
    {
        private int deaths;
        private int draws;
        private int exp;
        private int headshots;
        private int kills;
        private int loses;
        private int point;
        private int wins;
        public PROTOCOL_BASE_DAILY_RECORD_PAK()
        {
        }
        public PROTOCOL_BASE_DAILY_RECORD_PAK(int wins, int draws, int loses, int kills, int headshots, int deaths, int exp, int point)
        {
            this.wins = wins;
            this.draws = draws;
            this.loses = loses;
            this.kills = kills;
            this.headshots = headshots;
            this.deaths = deaths;
            this.exp = exp;
            this.point = point;
        }
        public override void Write()
        {
            WriteH(623); // TOTAL GAMES
            WriteH((short)wins); // WIN
            WriteH((short)draws); // DRAW
            WriteH((short)loses); // LOSES
            WriteH((short)kills); // KILL
            WriteH((short)headshots); // Headshots
            WriteH((short)deaths); // Death
            WriteD(exp); // EARN EXP
            WriteD(point); // EARN POINTS
            WriteD(0); // PLAY TIME IN SECONDS
            WriteC(0); // UNKNOWN
            WriteD(0); // PLAY TIME IN SECONDS
            WriteH(6); // UNKNOWN
        }
    }
}