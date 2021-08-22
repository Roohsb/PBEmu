using Core.models.account;
using Core.models.account.players;
using Core.server;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class FRIEND_MY_FRIENDLIST_PAK : SendPacket
    {
        private List<Friend> friends;
        public FRIEND_MY_FRIENDLIST_PAK(List<Friend> frie)
        {
            friends = frie;
        }

        public override void Write()
        {
            WriteH(274);
            WriteC((byte)friends.Count);
            for (int i = 0; i < friends.Count; i++)
            {
                Friend f = friends[i];
                PlayerInfo info = f.player;
                if (info == null)
                    WriteB(new byte[17]);
                else
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