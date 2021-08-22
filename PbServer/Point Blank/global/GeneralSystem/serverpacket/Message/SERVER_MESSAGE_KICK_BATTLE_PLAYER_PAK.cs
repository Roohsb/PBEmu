using Core.models.enums.errors;
using Core.server;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK : SendPacket
    {
        private EventErrorEnum _error;
        public SERVER_MESSAGE_KICK_BATTLE_PLAYER_PAK(EventErrorEnum error)
        {
            _error = error;
        }

        public override void Write()
        {
            WriteH(2052);
            WriteD((uint)_error);
        }
    }
}