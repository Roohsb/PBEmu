using Core;
using Core.managers;
using Core.models.account.clan;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CREATE_INVITE_REC : ReceiveGamePacket
    {
        private int clanId;
        private string text;
        private uint erro;
        public CLAN_CREATE_INVITE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            clanId = ReadD();
            text = ReadS(ReadC());
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                ClanInvite invite = new ClanInvite
                {
                    clan_id = clanId,
                    player_id = _client.player_id,
                    text = text,
                    inviteDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"))
                };
                if (p.clanId > 0 || p.player_name.Length == 0)
                    erro = 2147487836;
                else if (ClanManager.GetClan(clanId)._id == 0)
                    erro = 0x80000000;
                else if (PlayerManager.GetRequestCount(clanId) >= 100)
                    erro = 2147487831;
                else if (!PlayerManager.CreateInviteInDb(invite))
                    erro = 2147487848;
                invite = null;
                _client.SendPacket(new CLAN_CREATE_INVITE_PAK(erro, clanId));
            }
            catch (Exception ex)
            {
                Logger.Info("[CLAN_CREATE_INVITE_REC] " + ex.ToString());
            }
        }
    }
}