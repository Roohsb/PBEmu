﻿using Core;
using Core.models.account.clan;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CLOSE_REC : ReceiveGamePacket
    {
        private uint erro;
        public CLAN_CLOSE_REC(GameClient client, byte[] data)
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
                if (player != null)
                {
                    Clan clan = ClanManager.GetClan(player.clanId);
                    if (clan._id > 0 && clan.owner_id == _client.player_id &&
                        ComDiv.DeleteDB("clan_data", "clan_id", clan._id) &&
                        ComDiv.UpdateDB("accounts", "player_id", player.player_id, new string[] { "clan_id", "clanaccess", "clan_game_pt", "clan_wins_pt" }, 0, 0, 0, 0) &&
                        ClanManager.RemoveClan(clan))
                    {
                        player.clanId = 0;
                        player.clanAccess = 0;
                        SEND_CLAN_INFOS.Load(clan, 1);
                    }
                    else erro = 2147487850;
                }
                else erro = 2147487850;
                _client.SendPacket(new CLAN_CLOSE_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info("[CLAN_CLOSE_REC] " + ex.ToString());
            }
        }
    }
}