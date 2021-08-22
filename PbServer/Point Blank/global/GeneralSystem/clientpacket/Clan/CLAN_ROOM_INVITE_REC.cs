using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_ROOM_INVITED_REC : ReceiveGamePacket
    {
        private long pId;
        public CLAN_ROOM_INVITED_REC(GameClient client, byte[] data)
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
                if (player == null || player.clanId == 0)
                    return;
                Account member = AccountManager.GetAccount(pId, 0); //Não usar DB?
                if (member != null && member.clanId == player.clanId)
                    member.SendPacket(new CLAN_ROOM_INVITE_RESULT_PAK(_client.player_id), false);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}