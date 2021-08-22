using Core;
using Core.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_REQUEST_INFO_REC : ReceiveGamePacket
    {
        private long pId;
        public CLAN_REQUEST_INFO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            pId = ReadQ();
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                _client.SendPacket(new CLAN_REQUEST_INFO_PAK(pId, PlayerManager.GetRequestText(player.clanId, pId)));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}