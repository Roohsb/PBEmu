using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class AUTH_FIND_USER_PAK : SendPacket
    {
        private uint _erro;
        private Account player;
        public AUTH_FIND_USER_PAK(uint erro, Account player)
        {
            _erro = erro;
            this.player = player;
        }

        public override void Write()
        {
            WriteH(298);
            WriteD(_erro);
            if (_erro == 0)
            {
                WriteD(player._rank);
                WriteD(ComDiv.GetPlayerStatus(player._status, player._isOnline));
                WriteS(ClanManager.GetClan(player.clanId)._name, 17);
                WriteD(player._statistic.fights);
                WriteD(player._statistic.fights_win);
                WriteD(player._statistic.fights_lost);
                WriteD(player._statistic.fights_draw);
                WriteD(player._statistic.kills_count);
                WriteD(player._statistic.headshots_count);
                WriteD(player._statistic.deaths_count);
                WriteD(player._statistic.totalfights_count);
                WriteD(player._statistic.totalkills_count);
                WriteD(player._statistic.escapes);
                WriteD(player._statistic.fights);
                WriteD(player._statistic.fights_win);
                WriteD(player._statistic.fights_lost);
                WriteD(player._statistic.fights_draw);
                WriteD(player._statistic.kills_count);
                WriteD(player._statistic.headshots_count);
                WriteD(player._statistic.deaths_count);
                WriteD(player._statistic.totalfights_count);
                WriteD(player._statistic.totalkills_count);
                WriteD(player._statistic.escapes);
                WriteD(player._equip._primary);
                WriteD(player._equip._secondary);
                WriteD(player._equip._melee);
                WriteD(player._equip._grenade);
                WriteD(player._equip._special);
                WriteD(player._equip._red);
                WriteD(player._equip._blue);
                WriteD(player._equip._helmet);
                WriteD(player._equip._beret);
                WriteD(player._equip._dino);
                WriteH(0);
                WriteC(0);
            }
        }
    }
}