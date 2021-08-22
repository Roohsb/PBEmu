using Game.data.model;
using Core.server;

namespace Game.global.serverpacket
{
    public class GM_LOG_LOBBY_PAK : SendPacket
    {
        private Account player;
        public GM_LOG_LOBBY_PAK(Account p)
        {
            player = p;
        }

        public override void Write()
        {
            WriteH(2685);
            WriteD(0);
            WriteQ(player.player_id);
        }
    }
}