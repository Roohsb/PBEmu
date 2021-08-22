/*
 * Arquivo: CLAN_WAR_MATCH_TEAM_INFO_PAK.cs
 * Código criado pela MoMz Games
 * Última data de modificação: 05/02/2017
 * Sintam inveja, não nos atinge
 */

using Core.managers;
using Core.models.account.clan;
using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_WAR_MATCH_TEAM_INFO_PAK : SendPacket
    {
        private uint _erro;
        private Clan c;
        private Account leader;
        public CLAN_WAR_MATCH_TEAM_INFO_PAK(uint erro, Clan c)
        {
            _erro = erro;
            this.c = c;
            if (this.c != null)
            {
                leader = AccountManager.GetAccount(this.c.owner_id, 0);
                if (leader == null) _erro = 0x80000000;
            }
        }
        public CLAN_WAR_MATCH_TEAM_INFO_PAK(uint erro)
        {
            _erro = erro;
        }
        public override void Write()
        {
            WriteH(1570);
            WriteD(_erro);
            if (_erro == 0)
            {
                int players = PlayerManager.GetClanPlayers(c._id);
                WriteD(c._id);
                WriteS(c._name, 17);
                WriteC((byte)c._rank);
                WriteC((byte)players);
                WriteC((byte)c.maxPlayers);
                WriteD(c.creationDate);
                WriteD(c._logo);
                WriteC((byte)c._name_color);
                WriteC((byte)c.GetClanUnit(players));
                WriteD(c._exp);
                WriteD(0);
                WriteQ(c.owner_id);
                WriteS(leader.player_name, 33);
                WriteC((byte)leader._rank);
                WriteS("", 255);
            }//727 bytes
        }
    }
}