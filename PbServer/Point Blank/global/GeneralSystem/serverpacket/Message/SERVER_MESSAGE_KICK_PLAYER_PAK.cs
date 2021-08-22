using Core.server;

namespace Game.global.serverpacket
{
    public class SERVER_MESSAGE_KICK_PLAYER_PAK : SendPacket
    {
        public SERVER_MESSAGE_KICK_PLAYER_PAK()
        {
        }

        public override void Write()
        {
            WriteH(2051);
        }
    }
}