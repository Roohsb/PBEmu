using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_REQUEST_CONTEXT_REC : ReceiveGamePacket
    {
        public CLAN_REQUEST_CONTEXT_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                _client.SendPacket(new CLAN_REQUEST_CONTEXT_PAK(player.clanId));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}