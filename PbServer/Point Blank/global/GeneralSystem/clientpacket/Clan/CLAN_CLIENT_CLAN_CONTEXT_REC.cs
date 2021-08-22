using Core;
using Game.data.managers;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CLIENT_CLAN_CONTEXT_REC : ReceiveGamePacket
    {
        public CLAN_CLIENT_CLAN_CONTEXT_REC(GameClient client, byte[] data)
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
                _client.SendPacket(new CLAN_CLIENT_CLAN_CONTEXT_PAK(ClanManager._clans.Count));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}