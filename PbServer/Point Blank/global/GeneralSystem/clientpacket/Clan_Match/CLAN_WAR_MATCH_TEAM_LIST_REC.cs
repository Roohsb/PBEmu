using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_MATCH_TEAM_LIST_REC : ReceiveGamePacket
    {
        private int page;
        public CLAN_WAR_MATCH_TEAM_LIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            page = ReadH();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || p._match == null)
                    return;
                Channel ch = p.GetChannel();
                if (ch != null && ch._type == 4)
                    _client.SendPacket(new CLAN_WAR_MATCH_TEAM_LIST_PAK(page, ch._matchs, p._match._matchId));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}