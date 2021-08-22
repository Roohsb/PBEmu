using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_REQUEST_INFO_PAK : SendPacket
    {
        private string text;
        private uint _erro;
        private Account p;
        public CLAN_REQUEST_INFO_PAK(long id, string txt)
        {
            text = txt;
            p = AccountManager.GetAccount(id, 0);
            if (p == null || text == null)
                _erro = 0x80000000;
        }

        public override void Write()
        {
            WriteH(1325);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteQ(p.player_id);
                WriteS(p.player_name, 33);
                WriteC((byte)p._rank);
                WriteD(p._statistic.kills_count);
                WriteD(p._statistic.deaths_count);
                WriteD(p._statistic.fights);
                WriteD(p._statistic.fights_win);
                WriteD(p._statistic.fights_lost);
                WriteS(text, text.Length + 1);
            }
        }
    }
}