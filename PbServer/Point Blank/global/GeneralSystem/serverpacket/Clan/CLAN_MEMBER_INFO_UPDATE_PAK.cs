using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_MEMBER_INFO_UPDATE_PAK : SendPacket
    {
        private Account p;
        private ulong status;
        public CLAN_MEMBER_INFO_UPDATE_PAK(Account pl)
        {
            p = pl;
            status = ComDiv.GetClanStatus(pl._status, pl._isOnline);
        }

        public override void Write()
        {
            WriteH(1380);
            WriteQ(p.player_id);
            WriteS(p.player_name, 33);
            WriteC((byte)p._rank);
            WriteC((byte)p.clanAccess);
            WriteQ(status);
            WriteD(p.clanDate);
            WriteC((byte)p.name_color);
        }
    }
}