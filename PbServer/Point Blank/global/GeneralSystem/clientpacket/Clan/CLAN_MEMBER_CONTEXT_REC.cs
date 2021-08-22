using Core;
using Core.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_MEMBER_CONTEXT_REC : ReceiveGamePacket
    {
        public CLAN_MEMBER_CONTEXT_REC(GameClient client, byte[] data)
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
                Account p = _client._player;
                if (p == null)
                    return;
                int clanId = p.clanId;
                if (clanId == 0)
                    _client.SendPacket(new CLAN_MEMBER_CONTEXT_PAK(-1));
                else
                    _client.SendPacket(new CLAN_MEMBER_CONTEXT_PAK(0, PlayerManager.GetClanPlayers(clanId)));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}