using Core.models.account;
using Core.models.account.players;
using Core.models.enums.friends;
using Core.server;

namespace Game.global.Authentication
{
    public class FRIEND_UPDATE_PAK : SendPacket
    {
        private Friend _f;
        private int _index;
        private FriendState _state;
        private FriendChangeState _type;
        public FRIEND_UPDATE_PAK(FriendChangeState type, Friend friend, FriendState state, int idx)
        {
            _state = state;
            _f = friend;
            _type = type;
            _index = idx;
        }

        public override void Write()
        {
            WriteH(279);
            WriteC((byte)_type);
            WriteC((byte)_index);
            switch(_type)
            {
                case FriendChangeState @state when (@state == FriendChangeState.Insert || @state == FriendChangeState.Update):
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
                        break;
                    }
                default:
                    {
                        WriteB(new byte[17]); break;
                    }
            }
        }
    }
}