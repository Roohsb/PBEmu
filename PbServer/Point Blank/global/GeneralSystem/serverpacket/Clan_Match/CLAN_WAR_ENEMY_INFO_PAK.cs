using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_ENEMY_INFO_PAK : SendPacket
    {
        public Match mt;
        public CLAN_WAR_ENEMY_INFO_PAK(Match match)
        {
            mt = match;
        }

        public override void Write()
        {
            WriteH(1574);
            WriteH((short)mt.GetServerInfo());
            WriteC((byte)mt._matchId);
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
        }
    }
}