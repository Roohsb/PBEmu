﻿using Core.server;

namespace Core.models.account.players
{
    public class ItemsModel
    {
        public int _id, _category, _equip;
        public string _name;
        public long _objId;
        public uint _count;
        public ItemsModel DeepCopy() => (ItemsModel)MemberwiseClone();
        public ItemsModel()
        {
        }
        public ItemsModel(int id)
        {
            SetItemId(id);
        }
        public ItemsModel(int id, string name, int equip, uint count, long objId = 0)
        {
            _objId = objId;
            SetItemId(id);
            _name = name;
            _equip = equip;
            _count = count;
        }
        public ItemsModel(int id, int category, string name, int equip, uint count, long objId = 0)
        {
            _objId = objId;
            _id = id;
            _category = category;
            _name = name;
            _equip = equip;
            _count = count;
        }
        public ItemsModel(ItemsModel item)
        {
            _id = item._id;
            _category = item._category;
            _name = item._name;
            _count = item._count;
            _equip = item._equip;
        }
        public void SetItemId(int id)
        {
            _id = id;
            _category = ComDiv.GetItemCategory(id);
        }
    }
}