using Core.managers;
using Core.models.account.players;
using Core.models.enums.flags;
using Core.server;
using Game.data.model;

namespace Game.global.serverpacket
{
    public class INVENTORY_EQUIPED_ITEMS_PAK : SendPacket
    {
        private InventoryFlag type;
        private PlayerEquipedItems equip;
        /// <summary>
        /// Gera um pacote que faz uma checagem em todos os itens equipados, comparando-os com o inventário.
        /// </summary>
        /// <param name="p">Conta</param>
        public INVENTORY_EQUIPED_ITEMS_PAK(Account p)
        {
            type = (InventoryFlag)PlayerManager.CheckEquipedItems(p._equip, p._inventory._items);
            equip = p._equip;
        }
        public INVENTORY_EQUIPED_ITEMS_PAK(Account p, int type)
        {
            this.type = (InventoryFlag)type;
            equip = p._equip;
        }

        public override void Write()
        {
            WriteH(2058);
            WriteD((int)type);
            if (type.HasFlag(InventoryFlag.Character))
            {
                WriteD(equip._red);
                WriteD(equip._blue);
                WriteD(equip._helmet);
                WriteD(equip._beret);
                WriteD(equip._dino);
            }
            if (type.HasFlag(InventoryFlag.Weapon))
            {
                WriteD(equip._primary);
                WriteD(equip._secondary);
                WriteD(equip._melee);
                WriteD(equip._grenade);
                WriteD(equip._special);
            }
        }
    }
}