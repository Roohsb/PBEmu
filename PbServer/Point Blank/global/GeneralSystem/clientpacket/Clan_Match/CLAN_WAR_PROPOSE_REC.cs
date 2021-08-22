using Core;
using Core.models.enums.match;
using Game.data.model;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_PROPOSE_REC : ReceiveGamePacket
    {
        private int id, serverInfo;
        private uint erro;
        public CLAN_WAR_PROPOSE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            id = ReadH();
            serverInfo = ReadH();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p != null && p._match != null && p.matchSlot == p._match._leader && p._match._state == MatchState.Ready)
                {
                    Match mt = ChannelsXML.getChannel(serverInfo - ((serverInfo / 10) * 10)).GetMatch(id);
                    if (mt != null)
                    {
                        Account lider = mt.GetLeader();
                        if (lider != null && lider._connection != null && lider._isOnline)
                            lider.SendPacket(new CLAN_WAR_MATCH_REQUEST_BATTLE_PAK(p._match, p));
                        else
                            erro = 0x80000000;
                    }
                    else erro = 0x80000000;
                }
                else erro = 0x80000000;
                _client.SendPacket(new CLAN_WAR_MATCH_PROPOSE_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info("CLAN_WAR_PROPOSE_REC: " + ex.ToString());
            }
        }
    }
}