using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_CREATED_ROOM_PAK : SendPacket
    {
        public Match _mt;
        public CLAN_WAR_CREATED_ROOM_PAK(Match match)
        {
            _mt = match;
        }

        public override void Write()
        {
            WriteH(1564);
            WriteH((short)_mt._matchId);
            WriteD(_mt.GetServerInfo());
            WriteH((short)_mt.GetServerInfo());
            WriteC(10);
        }
    }
}