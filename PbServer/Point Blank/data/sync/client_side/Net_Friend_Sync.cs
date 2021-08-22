using Core.models.account;
using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.data.sync.client_side
{
    public class Net_Friend_Sync
    {
        public static void Load(ReceiveGPacket p)
        {
            long playerId = p.ReadQ();
            int type = p.ReadC();
            long friendId = p.ReadQ();
            Friend friendModel = null;

            if (type <= 1)
            {
                int state = p.ReadC();
                bool removed = p.ReadC() == 1;
                friendModel = new Friend(friendId) { state = state, removed = removed };
            }
            if (friendModel == null && type <= 1)
                return;

            Account player = AccountManager.GetAccount(playerId, true);
            if (player != null)
            {
                if (type <= 1)
                {
                    friendModel.player.player_name = player.player_name;
                    friendModel.player._rank = player._rank;
                    friendModel.player._isOnline = player._isOnline;
                    friendModel.player._status = player._status;
                }
                switch(type)
                {
                    case 0: player.FriendSystem.AddFriend(friendModel); break;
                    case 1:
                            Friend myFriend = player.FriendSystem.GetFriend(friendId);
                            if (myFriend != null)
                                 myFriend = friendModel;
                            break;
                    case 2:  player.FriendSystem.RemoveFriend(friendId); break;
                }
            }
        }
    }
}