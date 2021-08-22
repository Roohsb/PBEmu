using Core;
using Core.managers;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_PLAYER_CLEAN_INVITES_REC : ReceiveGamePacket
    {
        private uint erro;
        public CLAN_PLAYER_CLEAN_INVITES_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Run()
        {
            try
            {
                if (_client == null || !PlayerManager.DeleteInviteDb(_client.player_id))
                    erro = 2147487835;
                _client.SendPacket(new CLAN_PLAYER_CLEAN_INVITES_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }

        public override void Read()
        {
        }
    }
}