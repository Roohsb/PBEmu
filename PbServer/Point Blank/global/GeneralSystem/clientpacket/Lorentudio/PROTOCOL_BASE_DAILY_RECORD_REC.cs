using Core;
using Core.filters;
using Core.managers;
using Core.models.account.players;
using Core.xml;
using Game.data.managers;
using Game.data.model;
using Game.global.GeneralSystem.serverpacket.Lorenstudio;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket.Lorentudio
{
   public  class PROTOCOL_BASE_DAILY_RECORD_REC : ReceiveGamePacket
    {
        private PlayerDailyRecord Daily;

        public PROTOCOL_BASE_DAILY_RECORD_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            Account p = _client._player;
            Daily = PlayerManager.GetPlayerDailyRecord(p.player_id);
            _client.SendPacket(new PROTOCOL_BASE_DAILY_RECORD_PAK(Daily.Wins, Daily.Draws, Daily.Loses, Daily.Kills, Daily.Headshots, Daily.Deaths, Daily.Exp, Daily.Point));
        }
    }
}