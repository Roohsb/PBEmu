using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_MATCH_TEAM_CONTEXT_REC : ReceiveGamePacket
    {
        public CLAN_WAR_MATCH_TEAM_CONTEXT_REC(GameClient client, byte[] data)
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
                if (p == null || p._match == null)
                    return;
                Channel ch = p.GetChannel();
                if (ch != null && ch._type == 4)
                    _client.SendPacket(new CLAN_WAR_MATCH_TEAM_CONTEXT_PAK(ch._matchs.Count));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}