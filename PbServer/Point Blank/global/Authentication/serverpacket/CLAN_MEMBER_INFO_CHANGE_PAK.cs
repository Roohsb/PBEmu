using Core.models.enums.friends;
using Core.server;
using Game.data.model;

namespace Game.global.Authentication
{
    public class CLAN_MEMBER_INFO_CHANGE_PAK : SendPacket
    {
        private ulong status;
        private Account member;
        public CLAN_MEMBER_INFO_CHANGE_PAK(Account player)
        {
            member = player;
            status = ComDiv.GetClanStatus(player._status, player._isOnline);
        }
        public CLAN_MEMBER_INFO_CHANGE_PAK(Account player, FriendState st)
        {
            member = player;
            if (st == 0)
                status = ComDiv.GetClanStatus(player._status, player._isOnline);
            else
                status = ComDiv.GetClanStatus(st);
        }
        public override void Write()
        {
            WriteH(1355);
            WriteQ(member.player_id);
            WriteQ(status);
        }
    }
}