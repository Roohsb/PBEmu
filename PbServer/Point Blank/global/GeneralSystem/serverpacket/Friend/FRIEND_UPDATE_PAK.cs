using Core.models.account;
using Core.models.account.players;
using Core.models.enums.friends;
using Core.server;

namespace Game.global.serverpacket
{
    public class FRIEND_UPDATE_PAK : SendPacket
    {
        private Friend _f;
        private int _index;
        private FriendState _state;
        private FriendChangeState _type;
        public FRIEND_UPDATE_PAK(FriendChangeState type, Friend friend, int idx)
        {
            _type = type;
            _state = (FriendState)friend.state;
            _f = friend;
            _index = idx;
        }
        public FRIEND_UPDATE_PAK(FriendChangeState type, Friend friend, int state, int idx)
        {
            _type = type;
            _state = (FriendState)state;
            _f = friend;
            _index = idx;
        }
        public FRIEND_UPDATE_PAK(FriendChangeState type, Friend friend, FriendState state, int idx)
        {
            _type = type;
            _state = state;
            _f = friend;
            _index = idx;
        }
        public override void Write()
        {
            WriteH(279);
            WriteC((byte)_type); //Somente 1, 2 e 4.
            WriteC((byte)_index);
            if (_type == FriendChangeState.Insert || _type == FriendChangeState.Update)
            {
                PlayerInfo info = _f.player;
                if (info == null)
                    WriteB(new byte[17]);
                else
                {
                    WriteC((byte)(info.player_name.Length + 1));
                    WriteS(info.player_name, info.player_name.Length + 1);
                    WriteQ(_f.player_id);
                    WriteD(ComDiv.GetFriendStatus(_f, _state));
                    WriteC((byte)info._rank);
                    WriteC(0);
                    WriteH(0);
                }
            }
            else
                WriteB(new byte[17]);
        }
    }
}