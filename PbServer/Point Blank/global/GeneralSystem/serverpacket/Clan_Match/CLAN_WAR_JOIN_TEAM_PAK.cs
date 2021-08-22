using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_JOIN_TEAM_PAK : SendPacket
    {
        private Match m;
        private uint _erro;
        public CLAN_WAR_JOIN_TEAM_PAK(uint erro, Match m = null)
        {
            _erro = erro;
            this.m = m;
        }
        public override void Write()
        {
            WriteH(1549);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteH((short)m._matchId);
                WriteH((ushort)m.GetServerInfo());
                WriteH((ushort)m.GetServerInfo());
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
                for (int i = 0; i < m.formação; i++)
                {
                    SLOT_MATCH s = m._slots[i];
                    Account pS = m.GetPlayerBySlot(s);
                    if (pS != null)
                    {
                        WriteC((byte)pS._rank);
                        WriteS(pS.player_name, 33);
                        WriteQ(pS.player_id);
                        WriteC((byte)s.state);
                    }
                    else
                        WriteB(new byte[43]);
                }
            }
        }
    }
}