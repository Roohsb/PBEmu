using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class LOBBY_GET_PLAYERINFO2_PAK : SendPacket
    {
        private Account ac;
        public LOBBY_GET_PLAYERINFO2_PAK(long player)
        {
            ac = AccountManager.GetAccount(player, true);
        }

        public override void Write()
        {
            WriteH(3100);
            if (ac != null && ac._equip != null)
            {
                WriteD(ac._equip._primary);
                WriteD(ac._equip._secondary);
                WriteD(ac._equip._melee);
                WriteD(ac._equip._grenade);
                WriteD(ac._equip._special);
                WriteD(ac._equip._red);
                WriteD(ac._equip._blue);
                WriteD(ac._equip._helmet);
                WriteD(ac._equip._beret);
                WriteD(ac._equip._dino);
            }
            else
            {
                WriteD(0);
                WriteD(601002003);
                WriteD(702001001);
                WriteD(803007001);
                WriteD(904007002);
                WriteD(1001001005);
                WriteD(1001002006);
                WriteD(1102003001);
                WriteD(0);
                WriteD(1006003041);
            }
            WriteD(0); //Count de writeD
        }
    }
}