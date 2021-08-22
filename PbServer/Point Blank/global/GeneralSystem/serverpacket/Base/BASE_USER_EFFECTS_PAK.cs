/*
 * Arquivo: PLAYER_CONFIG_PAK.cs
 * Código criado pela MoMz Games
 * Última data de modificação: 11/01/2017
 * Sintam inveja, não nos atinge
 */

using Core.models.account.players;
using Core.server;

namespace Game.global.serverpacket
{
    public class BASE_USER_EFFECTS_PAK : SendPacket
    {
        private int _type;
        private PlayerBonus _bonus;
        public BASE_USER_EFFECTS_PAK(int type, PlayerBonus bonus)
        {
            _type = type;
            _bonus = bonus;
        }

        public override void Write()
        {
            WriteH(2638);
            WriteH((ushort)_type);
            WriteD(_bonus.fakeRank);
            WriteD(_bonus.fakeRank);
            WriteS(_bonus.fakeNick, 33);
            WriteH((short)_bonus.sightColor);
        }
    }
}