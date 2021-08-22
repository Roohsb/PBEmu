using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.data.sync.client_side
{
    public static class Net_Clan_Sync
    {
        public static void Load(ReceiveGPacket p)
        {
            long playerId = p.ReadQ();
            int type = p.ReadC();
            Account player = AccountManager.GetAccount(playerId, true);
            if (player == null)
                return;

            if (type == 3)
            {
                player.clanId = p.ReadD();
                player.clanAccess = p.ReadC();
            }
        }
    }
}