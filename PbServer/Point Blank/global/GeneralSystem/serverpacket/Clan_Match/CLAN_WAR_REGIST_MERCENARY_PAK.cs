using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_REGIST_MERCENARY_PAK : SendPacket
    {
        private Match m;
        public CLAN_WAR_REGIST_MERCENARY_PAK(Match m)
        {
            this.m = m;
        }

        public override void Write()
        {
            WriteH(1552);
            WriteH((short)m.GetServerInfo());
            WriteC((byte)m._state);
            WriteC((byte)m.friendId);
            WriteC((byte)m.formação);
            WriteC((byte)m.GetCountPlayers());
            WriteD(m._leader);
            WriteC(0);
            foreach (SLOT_MATCH s in m._slots)
            {
                Account p = m.GetPlayerBySlot(s);
                if (p != null)
                {
                    WriteC((byte)p._rank);
                    WriteS(p.player_name, 33);
                    WriteQ(s._playerId);
                    WriteC((byte)s.state);
                }
                else WriteB(new byte[43]);
            }
        }
    }
}