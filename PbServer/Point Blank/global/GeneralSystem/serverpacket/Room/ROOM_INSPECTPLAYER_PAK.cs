using Core.models.account.players;
using Core.server;
using Game.data.model;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class ROOM_INSPECTPLAYER_PAK : SendPacket
    {
        private Account p;
        public ROOM_INSPECTPLAYER_PAK(Account p)
        {
            this.p = p;
        }

        public override void Write()
        {
            WriteH(3893);
            WriteD(p._equip._primary);
            WriteD(p._equip._secondary);
            WriteD(p._equip._melee);
            WriteD(p._equip._grenade);
            WriteD(p._equip._special);
            WriteD(p._equip._red);
            WriteD(p._equip._blue);
            WriteD(p._equip._helmet);
            WriteD(p._equip._beret);
            WriteD(p._equip._dino);
            List<ItemsModel> cupons = p._inventory.getItemsByType(4);
            WriteD(cupons.Count);
            for (int i = 0; i < cupons.Count; i++)
                WriteD(cupons[i]._id);
        }
    }
}