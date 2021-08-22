using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_MATCH_REQUEST_BATTLE_PAK : SendPacket
    {
        public Match mt;
        public Account p;
        public CLAN_WAR_MATCH_REQUEST_BATTLE_PAK(Match match, Account p)
        {
            this.mt = match;
            this.p = p;
        }

        public override void Write()
        {
            WriteH(1555);
            WriteH((short)mt._matchId);
            WriteH((ushort)mt.GetServerInfo());
            WriteH((ushort)mt.GetServerInfo());
            WriteC((byte)mt._state);
            WriteC((byte)mt.friendId);
            WriteC((byte)mt.formação);
            WriteC((byte)mt.GetCountPlayers());
            WriteD(mt._leader);
            WriteC(0);
            WriteD(mt.clan._id);
            WriteC((byte)mt.clan._rank);
            WriteD(mt.clan._logo);
            WriteS(mt.clan._name, 17);
            WriteT(mt.clan._pontos);
            WriteC((byte)mt.clan._name_color);
            if (p != null)
            {
                WriteC((byte)p._rank);
                WriteS(p.player_name, 33);
                WriteQ(p.player_id);
                WriteC((byte)mt._slots[mt._leader].state);
            }
            else
                WriteB(new byte[43]);
        }
    }
}