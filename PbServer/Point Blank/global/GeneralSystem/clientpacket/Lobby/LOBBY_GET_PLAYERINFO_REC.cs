using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_GET_PLAYERINFO_REC : ReceiveGamePacket
    {
        private uint sessionId;
        public LOBBY_GET_PLAYERINFO_REC(GameClient client, byte[] data)
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
            Account p = null;
            try
            {
                p = AccountManager.GetAccount(player.GetChannel().GetPlayer(sessionId)._playerId, true);
            }
            catch { }
            _client.SendPacket(new LOBBY_GET_PLAYERINFO_PAK(p?._statistic));
        }
    }
}