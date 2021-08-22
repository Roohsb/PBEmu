using Core.models.account;
using Core.models.account.players;
using Core.server;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_FRIENDS_PAK : SendPacket
    {
        private List<Friend> friends;
        public BASE_USER_FRIENDS_PAK(List<Friend> friends)
        {
            this.friends = friends;
        }

        public override void Write()
        {
            WriteH(274);
            WriteC((byte)friends.Count);
            for (int i = 0; i < friends.Count; i++)
            {
                Friend f = friends[i];
                PlayerInfo info = f.player;
                if(info != null)
                {
                    WriteC((byte)(info.player_name.Length + 1));
                    WriteS(info.player_name, info.player_name.Length + 1);
                    WriteQ(f.player_id);
                    WriteD(ComDiv.GetFriendStatus(f));
                    WriteC((byte)info._rank);
                    WriteH(0);
                    WriteC(0);
                }
            }
        }
    }
}