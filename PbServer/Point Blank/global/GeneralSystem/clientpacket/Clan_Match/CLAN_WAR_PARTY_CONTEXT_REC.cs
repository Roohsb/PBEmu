using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_PARTY_CONTEXT_REC : ReceiveGamePacket
    {
        private int matchs;
        public CLAN_WAR_PARTY_CONTEXT_REC(GameClient client, byte[] data)
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
                if (p != null && p.clanId > 0)
                {
                    Channel ch = p.GetChannel();
                    if (ch != null && ch._type == 4)
                    {
                        lock (ch._matchs)
                        {
                            for (int i = 0; i < ch._matchs.Count; i++)
                            {
                                Match m = ch._matchs[i];
                                if (m.clan._id == p.clanId)
                                    matchs++;
                            }
                        }
                    }
                }
                _client.SendPacket(new CLAN_WAR_PARTY_CONTEXT_PAK(matchs));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}