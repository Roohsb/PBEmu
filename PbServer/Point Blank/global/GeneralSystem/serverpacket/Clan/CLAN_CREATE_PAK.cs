using Core.managers;
using Core.models.account.clan;
using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class CLAN_CREATE_PAK : SendPacket
    {
        private Account _p;
        private Clan clan;
        private uint _erro;
        public CLAN_CREATE_PAK(uint erro, Clan clan, Account player)
        {
            _erro = erro;
            this.clan = clan;
            _p = player;
        }

        public override void Write()
        {
            WriteH(1311);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteD(clan._id);
                WriteS(clan._name, 17);
                WriteC((byte)clan._rank);
                WriteC((byte)PlayerManager.GetClanPlayers(clan._id));
                WriteC((byte)clan.maxPlayers);
                WriteD(clan.creationDate);
                WriteD(clan._logo);
                WriteB(new byte[10]);
                WriteQ(clan.owner_id);
                WriteS(_p.player_name, 33);
                WriteC((byte)_p._rank);
                WriteS(clan._info, 255);
                WriteS("Temp", 21);
                WriteC((byte)clan.limite_rank);
                WriteC((byte)clan.limite_idade);
                WriteC((byte)clan.limite_idade2);
                WriteC((byte)clan.autoridade);
                WriteS("", 255);
                WriteB(new byte[104]);
                WriteT(clan._pontos);
                WriteD(_p._gp);
            }
        }
    }
}