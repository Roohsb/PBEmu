using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_PARTY_LIST_REC : ReceiveGamePacket
    {
        private List<Match> partyList = new List<Match>();
        private int page;
        public CLAN_WAR_PARTY_LIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            page = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                if (p.clanId > 0)
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
                                    partyList.Add(m);
                            }
                        }
                    }
                }
                _client.SendPacket(new CLAN_WAR_PARTY_LIST_PAK(p.clanId == 0 ? 91 : 0, partyList));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}