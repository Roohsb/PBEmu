using Core.models.account.title;
using Core.models.room;
using Core.server;

namespace Game.global.serverpacket
{
    public class BATTLE_READYBATTLE2_PAK : SendPacket
    {
        private SLOT slot;
        private PlayerTitles title;
        public BATTLE_READYBATTLE2_PAK(SLOT slot, PlayerTitles title)
        {
            this.slot = slot;
            this.title = title;
        }

        public override void Write()
        {
            if (slot._equip == null)
                return;
            WriteH(3427);
            WriteC((byte)slot._id);
            WriteD(slot._equip._red);
            WriteD(slot._equip._blue);
            WriteD(slot._equip._helmet);
            WriteD(slot._equip._beret);
            WriteD(slot._equip._dino);
            WriteD(slot._equip._primary);
            WriteD(slot._equip._secondary);
            WriteD(slot._equip._melee);
            WriteD(slot._equip._grenade);
            WriteD(slot._equip._special);
            WriteD(0);
            WriteC((byte)title.Equiped1);
            WriteC((byte)title.Equiped2);
            WriteC((byte)title.Equiped3);
            if (LoginManager.Config.ClientVersion == "1.15.42")
                WriteD(0); //Somente 1.15.42
        }
    }
}