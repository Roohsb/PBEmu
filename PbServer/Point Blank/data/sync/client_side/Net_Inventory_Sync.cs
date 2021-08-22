using Core.models.account.players;
using Core.server;
using Game.data.managers;
using Game.data.model;

namespace Game.data.sync.client_side
{
    public class Net_Inventory_Sync
    {
        public static void Load(ReceiveGPacket p)
        {
            long playerId = p.ReadQ();
            long objId = p.ReadQ();
            int itemid = p.ReadD();
            int equip = p.ReadC();
            int category = p.ReadC();
            uint count = p.ReadUD();
            Account player = AccountManager.GetAccount(playerId, true);
            if (player == null)
                return;
            ItemsModel item = player._inventory.getItem(objId);
            if (item == null)
                player._inventory.AddItem(new ItemsModel { _objId = objId, _id = itemid, _equip = equip, _count = count, _category = category, _name = "" });
            else
                item._count = count;
        }
    }
}