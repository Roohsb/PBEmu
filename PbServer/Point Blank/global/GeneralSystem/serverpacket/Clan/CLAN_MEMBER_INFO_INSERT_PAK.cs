using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_MEMBER_INFO_INSERT_PAK : SendPacket
    {
        private Account p;
        private ulong status;
        public CLAN_MEMBER_INFO_INSERT_PAK(Account pl)
        {
            p = pl;
            status = ComDiv.GetClanStatus(pl._status, pl._isOnline);
        }

        public override void Write()
        {
            WriteH(1351);
            WriteC((byte)(p.player_name.Length + 1));
            WriteS(p.player_name, p.player_name.Length + 1);
            WriteQ(p.player_id);
            WriteQ(status);
            WriteC((byte)p._rank);
        }
    }
}