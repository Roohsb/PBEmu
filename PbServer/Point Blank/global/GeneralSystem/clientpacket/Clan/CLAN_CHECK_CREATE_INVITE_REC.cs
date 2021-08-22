using Core;
using Core.models.account.clan;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CHECK_CREATE_INVITE_REC : ReceiveGamePacket
    {
        private int clanId;
        private uint erro;
        public CLAN_CHECK_CREATE_INVITE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            clanId = ReadD();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Clan c = ClanManager.GetClan(clanId);
                if (c._id == 0)
                    erro = 0x80000000;
                else if (c.limite_rank > p._rank)
                    erro = 2147487867;
                _client.SendPacket(new CLAN_CHECK_CREATE_INVITE_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info("CLAN_CHECK_CREATE_INVITE_REC: " + ex.ToString());
            }
        }
    }
}