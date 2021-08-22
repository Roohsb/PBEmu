using Core.server;
using Game.data.model;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_MATCH_TEAM_LIST_PAK : SendPacket
    {
        private List<Match> matchs;
        private int myMatchIdx, _page, MatchCount;
        public CLAN_WAR_MATCH_TEAM_LIST_PAK(int page, List<Match> matchs, int matchId)
        {
            _page = page;
            myMatchIdx = matchId;
            MatchCount = (matchs.Count - 1);
            this.matchs = matchs;
        }

        public override void Write()
        {
            WriteH(1545);
            WriteH((ushort)MatchCount);//Quantidade de clãs na lista
            if (MatchCount == 0)
                return;
            WriteH(1);
            WriteH(0);
            WriteC((byte)MatchCount); //Quantidade de itens da lista a ser lida
            for (int i = 0; i < matchs.Count; i++)
            {
                Match m = matchs[i];
                if (m._matchId == myMatchIdx)
                    continue;
                WriteH((short)m._matchId);
                WriteH((short)m.GetServerInfo());
                WriteH((short)m.GetServerInfo());
                WriteC((byte)m._state);
                WriteC((byte)m.friendId);
                WriteC((byte)m.formação);
                WriteC((byte)m.GetCountPlayers());
                WriteD(m._leader);
                WriteC(0);
                WriteD(m.clan._id);
                WriteC((byte)m.clan._rank);
                WriteD(m.clan._logo);
                WriteS(m.clan._name, 17);
                WriteT(m.clan._pontos);
                WriteC((byte)m.clan._name_color);
                Account p = m.GetLeader();
                if (p != null)
                {
                    WriteC((byte)p._rank);
                    WriteS(p.player_name, 33);
                    WriteQ(p.player_id);
                    WriteC((byte)m._slots[m._leader].state);
                }
                else WriteB(new byte[43]);
            }
        }
    }
}