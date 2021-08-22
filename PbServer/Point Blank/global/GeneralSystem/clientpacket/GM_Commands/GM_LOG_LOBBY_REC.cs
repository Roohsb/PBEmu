using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class GM_LOG_LOBBY_REC : ReceiveGamePacket
    {
        private uint sessionId;
        public GM_LOG_LOBBY_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            sessionId = ReadUD();
        }

        public override void Run()
        {
            Account player = _client._player;
            if (player == null || !player.IsGM())
                return;
            Account p = null;
            try
            {
                p = AccountManager.GetAccount(player.GetChannel().GetPlayer(sessionId)._playerId, true);
            }
            catch { }
            if (p != null)
                _client.SendPacket(new GM_LOG_LOBBY_PAK(p));
        }
    }
}