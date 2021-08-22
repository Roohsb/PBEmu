using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_GET_PLAYERINFO2_REC : ReceiveGamePacket
    {
        private uint sessionId;
        public LOBBY_GET_PLAYERINFO2_REC(GameClient client, byte[] data)
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
            if (player == null)
                return;
            long playerId = 0;
            try
            {
                playerId = player.GetChannel().GetPlayer(sessionId)._playerId;
            }
            catch { }
            _client.SendPacket(new LOBBY_GET_PLAYERINFO2_PAK(playerId));
        }
    }
}