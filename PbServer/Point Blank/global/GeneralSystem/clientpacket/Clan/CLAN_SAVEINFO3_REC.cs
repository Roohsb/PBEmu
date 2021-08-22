using Core;
using Core.managers;
using Core.models.account.clan;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_SAVEINFO3_REC : ReceiveGamePacket
    {
        private int limite_rank, limite_idade, limite_idade2, autoridade;
        private uint erro;
        public CLAN_SAVEINFO3_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            autoridade = ReadC();
            limite_rank = ReadC();
            limite_idade = ReadC();
            limite_idade2 = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                Clan c = ClanManager.GetClan(player.clanId);
                if (c._id > 0 && (c.owner_id == _client.player_id) && PlayerManager.UpdateClanInfo(c._id, autoridade, limite_rank, limite_idade, limite_idade2))
                {
                    c.autoridade = autoridade;
                    c.limite_rank = limite_rank;
                    c.limite_idade = limite_idade;
                    c.limite_idade2 = limite_idade2;

                    //CADE A DATA GIGA???????????????????????????????????????? VIADO
                }
                else erro = 0x80000000;

                _client.SendPacket(new CLAN_SAVEINFO3_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}