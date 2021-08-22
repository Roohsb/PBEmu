using Core;
using Game.data.model;
using Game.data.xml;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_JOIN_TEAM_REC : ReceiveGamePacket
    {
        private int matchId, serverInfo, type;
        private uint erro;
        public CLAN_WAR_JOIN_TEAM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            matchId = ReadH();
            serverInfo = ReadH();
            type = ReadC(); //0 normal | 1 - amigos do clã
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (type >= 2 || p == null || p._match != null || p._room != null)
                {
                    _client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x80000000));
                    return;
                }
                int channelId = serverInfo - ((serverInfo / 10) * 10);
                Channel ch = ChannelsXML.getChannel(type == 0 ? channelId : p.channelId);
                if (ch != null)
                {
                    if (p.clanId == 0)
                        _client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x8000105B));
                    else
                    {
                        Match mt = type == 1 ? ch.GetMatch(matchId, p.clanId) : ch.GetMatch(matchId);
                        if (mt != null)
                            JoinPlayer(p, mt);
                        else
                            _client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x80000000));
                    }
                }
                else
                    _client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(0x80000000));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
        private void JoinPlayer(Account p, Match mt)
        {
            if (!mt.AddPlayer(p))
                erro = 0x80000000;
            _client.SendPacket(new CLAN_WAR_JOIN_TEAM_PAK(erro, mt));
            if (erro == 0)
            {
                using (CLAN_WAR_REGIST_MERCENARY_PAK packet = new CLAN_WAR_REGIST_MERCENARY_PAK(mt))
                    mt.SendPacketToPlayers(packet);
            }
        }
    }
}