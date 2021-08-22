using Core.server;
using Game.data.model;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_CLAN_MEMBERS_PAK : SendPacket
    {
        private List<Account> _players;
        public BASE_USER_CLAN_MEMBERS_PAK(List<Account> players)
        {
            _players = players;
        }

        public override void Write()
        {
            WriteH(1349);
            WriteC((byte)_players.Count);
            for (int i = 0; i < _players.Count; i++)
            {
                Account member = _players[i];
                WriteC((byte)(member.player_name.Length + 1));
                WriteS(member.player_name, member.player_name.Length + 1);
                WriteQ(member.player_id);
                WriteQ(ComDiv.GetClanStatus(member._status, member._isOnline));
                WriteC((byte)member._rank);
            }
        }
    }
}