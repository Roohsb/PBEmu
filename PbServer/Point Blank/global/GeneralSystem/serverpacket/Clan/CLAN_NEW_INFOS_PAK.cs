/*
 * Arquivo: CLAN_NEW_INFOS_PAK.cs
 * Código criado pela MoMz Games
 * Última data de modificação: 26/07/2017
 * Sintam inveja, não nos atinge
 */

using Core.models.account.clan;
using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_NEW_INFOS_PAK : SendPacket
    {
        private Clan clan;
        private Account p;
        private int players;
        public CLAN_NEW_INFOS_PAK(Clan c, Account owner, int clanPlayers)
        {
            clan = c;
            p = owner;
            players = clanPlayers;
        }
        public CLAN_NEW_INFOS_PAK(Clan c, int clanPlayers)
        {
            clan = c;
            p = AccountManager.GetAccount(clan.owner_id, 0);
            players = clanPlayers;
        }
        public override void Write()
        {
            WriteH(1328);
            WriteD(clan._id);
            WriteS(clan._name, 17);
            WriteC((byte)clan._rank);
            WriteC((byte)players);
            WriteC((byte)clan.maxPlayers);
            WriteD(clan.creationDate);
            WriteD(clan._logo);
            WriteC((byte)clan._name_color);
            WriteC((byte)clan.GetClanUnit(players));
            WriteD(clan._exp);
            WriteD(0);
            WriteQ(clan.owner_id);
            WriteS(p != null ? p.player_name : "", 33);
            WriteC((byte)(p != null ? p._rank : 0));
            WriteS(clan._info, 255);
            WriteS("Temp", 21);
            WriteC((byte)clan.limite_rank);
            WriteC((byte)clan.limite_idade);
            WriteC((byte)clan.limite_idade2);
            WriteC((byte)clan.autoridade);
            WriteS(clan._news, 255);
            WriteD(clan.partidas);
            WriteD(clan.vitorias);
            WriteD(clan.derrotas);
            WriteD(clan.partidas);
            WriteD(clan.vitorias);
            WriteD(clan.derrotas);
            //MELHORES MEMBROS DO CLÃ
            WriteQ(clan.BestPlayers.Exp.PlayerId); //XP Adquirida (Total)
            WriteQ(clan.BestPlayers.Exp.PlayerId); //XP Adquirida (Temporada)
            WriteQ(clan.BestPlayers.Wins.PlayerId); //Vitória (Total)
            WriteQ(clan.BestPlayers.Wins.PlayerId); //Vitória (Temporada)
            WriteQ(clan.BestPlayers.Kills.PlayerId); //Kills (Total)
            WriteQ(clan.BestPlayers.Kills.PlayerId); //Kills (Temporada)
            WriteQ(clan.BestPlayers.Headshot.PlayerId); //Headshots (Total)
            WriteQ(clan.BestPlayers.Headshot.PlayerId); //Headshots (Temporada)
            WriteQ(clan.BestPlayers.Participation.PlayerId); //Participação (Total)
            WriteQ(clan.BestPlayers.Participation.PlayerId); //Participação (Temporada)
            WriteT(clan._pontos);
        }
    }
}