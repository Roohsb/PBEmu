using Core.DB_Battle;
using Core.models.shop;
using Core.server;

using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Core.managers
{
    public static class ShopManager
    {
        public static List<GoodItem> ShopAllList = new List<GoodItem>(), ShopBuyableList = new List<GoodItem>();
        public static SortedList<int, GoodItem> ShopUniqueList = new SortedList<int, GoodItem>();
        public static List<ShopData> ShopDataMt1 = new List<ShopData>(), ShopDataMt2 = new List<ShopData>(), ShopDataMt3 = new List<ShopData>(), ShopDataGoods = new List<ShopData>(), ShopDataItems = new List<ShopData>();
        public static int TotalGoods, TotalItems, TotalMatching1, TotalMatching2, TotalMatching3, set4p;
        public static GoodItem good;
        public static void Load(int type, bool friday)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = "SELECT * FROM shop";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            good = new GoodItem
                            {
                                id = data.GetInt32(0),
                                price_gold = data.GetInt32(3),
                                price_cash = data.GetInt32(4),
                                auth_type = data.GetInt32(6),
                                buy_type2 = data.GetInt32(7),
                                buy_type3 = data.GetInt32(8),
                                tag = data.GetInt32(9),
                                title = data.GetInt32(10),
                                visibility = data.GetInt32(11)
                            };
                            good._item.SetItemId(data.GetInt32(1));
                            good._item._name = data.GetString(2);
                            good._item._count = (uint)data.GetInt32(5);
                            switch (type)
                            {
                                case int Tipos when (Tipos == 1 || Tipos == 2 && ComDiv.GetIdStatics(good._item._id, 1) == 12):
                                    {
                                        ShopAllList.Add(good);
                                        if (good.visibility != 2 && good.visibility != 4)
                                            ShopBuyableList.Add(good);
                                        if (!ShopUniqueList.ContainsKey(good._item._id) && good.auth_type > 0)
                                        {
                                            ShopUniqueList.Add(good._item._id, good);
                                            if (good.tag == 4 || good.tag == 8) 
                                                set4p++;
                                        }
                                        break;
                                    }
                            }

                        }
                        data.Close();
                    }
                    if (type == 1)
                    {
                        LoadDataMatching1Goods(0, friday);
                        LoadDataMatching2(1);
                        LoadDataMatching3(1);
                        LoadDataItems();
                    }
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                if (set4p > 0)
                    Logger.GameSystem(" [System] existe " + set4p + " itens para VIPs.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public static void Reset()
        {
            set4p = 0;
            ShopAllList.Clear();
            ShopBuyableList.Clear();
            ShopUniqueList.Clear();
            ShopDataMt1.Clear();
            ShopDataMt2.Clear();
            ShopDataMt3.Clear();
            ShopDataGoods.Clear();
            ShopDataItems.Clear();
            TotalGoods = 0;
            TotalItems = 0;
            TotalMatching1 = 0;
            TotalMatching2 = 0;
            TotalMatching3 = 0;
        }
        private static void LoadDataMatching1Goods(int cafe, bool blackfriday)
        {
            List<GoodItem> matchs = new List<GoodItem>(),
                goods = new List<GoodItem>();
            lock (ShopAllList)
            {
                for (int i = 0; i < ShopAllList.Count; i++)
                {
                    GoodItem good = ShopAllList[i];
                    if (good._item._count == 0)
                        continue;
                    if (good.tag != 8 && good.tag != 4 && good.visibility != 2 && good.visibility != 4)
                    {
                        if (blackfriday && good.tag == 0)
                        {
                            good.price_gold = (good.price_gold * 50) / 100;
                            good.price_cash = (good.price_cash * 50) / 100;
                            good.tag = 5;
                        }
                        matchs.Add(good);
                    }
                    if (good.visibility < 2 || good.visibility == 4)
                        goods.Add(good);
                }
            }
            TotalMatching1 = matchs.Count;
            TotalGoods = goods.Count;
            int Pages = (int)Math.Ceiling(matchs.Count / 741d);
            int count = 0;
            for (int i = 0; i < Pages; i++)
            {
                ShopDataMt1.Add(new ShopData
                {
                    Buffer = GetMatchingData(741, i, ref count, matchs),
                    ItemsCount = count,
                    Offset = (i * 741)
                });
            }
            Pages = (int)Math.Ceiling(goods.Count / 592d);
            for (int i = 0; i < Pages; i++)
            {
                ShopDataGoods.Add(new ShopData
                {
                    Buffer = GetGoodsData(592, i, ref count, goods),
                    ItemsCount = count,
                    Offset = (i * 592)
                });
            }
        }
        private static void LoadDataMatching2(int cafe)
        {
            List<GoodItem> matchs = new List<GoodItem>();
            lock (ShopAllList)
            {
                for (int i = 0; i < ShopAllList.Count; i++)
                {
                    GoodItem good = ShopAllList[i];
                    if (good._item._count == 0)
                        continue;
                    if (good.tag == 4 || (good.visibility != 2 && good.tag != 8))
                        matchs.Add(good);
                }
            }
            TotalMatching2 = matchs.Count;
            int Pages = (int)Math.Ceiling(matchs.Count / 741d);
            int count = 0;
            for (int i = 0; i < Pages; i++)
            {
                ShopDataMt2.Add(new ShopData
                {
                    Buffer = GetMatchingData(741, i, ref count, matchs),
                    ItemsCount = count,
                    Offset = (i * 741)
                });
            }
        }
        private static void LoadDataMatching3(int cafe)
        {
            List<GoodItem> matchs = new List<GoodItem>();
            lock (ShopAllList)
            {
                for (int i = 0; i < ShopAllList.Count; i++)
                {
                    GoodItem good = ShopAllList[i];
                    if (good._item._count == 0)
                        continue;
                    if (good.tag == 8 || (good.visibility != 2 && good.tag != 4))
                    {
                        good.tag = 4;
                        matchs.Add(good);
                    }

                }
            }
            TotalMatching3 = matchs.Count;
            int Pages = (int)Math.Ceiling(matchs.Count / 741d);
            int count = 0;
            for (int i = 0; i < Pages; i++)
            {
                ShopDataMt3.Add(new ShopData
                {
                    Buffer = GetMatchingData(741, i, ref count, matchs),
                    ItemsCount = count,
                    Offset = (i * 741)
                });
            }
        }
        private static void LoadDataItems()
        {
            List<GoodItem> items = new List<GoodItem>();
            lock (ShopUniqueList)
            {
                for (int i = 0; i < ShopUniqueList.Values.Count; i++)
                {
                    GoodItem good = ShopUniqueList.Values[i];
                    if (good.visibility != 1 && good.visibility != 3)
                        items.Add(good);
                }
            }
            TotalItems = items.Count;
            int ItemsPages = (int)Math.Ceiling(items.Count / 1111d);
            int count = 0;
            for (int i = 0; i < ItemsPages; i++)
            {
                ShopDataItems.Add(new ShopData
                {
                    Buffer = GetItemsData(1111, i, ref count, items),
                    ItemsCount = count,
                    Offset = (i * 1111)
                });
            }
        }
        private static byte[] GetItemsData(int maximum, int page, ref int count, List<GoodItem> list)
        {
            count = 0;
            using (SendGPacket p = new SendGPacket())
            {
                for (int i = (page * maximum); i < list.Count; i++)
                {
                    WriteItemsData(list[i], p);
                    if (++count == maximum)
                        break;
                }
                return p.mstream.ToArray();
            }
        }
        private static byte[] GetGoodsData(int maximum, int page, ref int count, List<GoodItem> list)
        {
            count = 0;
            using (SendGPacket p = new SendGPacket())
            {
                for (int i = (page * maximum); i < list.Count; i++)
                {
                    WriteGoodsData(list[i], p);
                    if (++count == maximum)
                        break;
                }
                return p.mstream.ToArray();
            }
        }
        private static byte[] GetMatchingData(int maximum, int page, ref int count, List<GoodItem> list)
        {
            count = 0;
            using (SendGPacket p = new SendGPacket())
            {
                for (int i = (page * maximum); i < list.Count; i++)
                {
                    WriteMatchData(list[i], p);
                    if (++count == maximum)
                        break;
                }
                return p.mstream.ToArray();
            }
        }
        private static void WriteItemsData(GoodItem good, SendGPacket p)
        {
            p.writeD(good._item._id);
            p.writeC((byte)good.auth_type);
            p.writeC((byte)good.buy_type2);
            p.writeC((byte)good.buy_type3);
            p.writeC((byte)good.title);
        }
        private static void WriteGoodsData(GoodItem good, SendGPacket p)
        {
            p.writeD(good.id);
            p.writeC(1);
            p.writeC((byte)(good.visibility == 4 ? 4 : 1));
            p.writeD(good.price_gold);
            p.writeD(good.price_cash);
            if(good.tag == 8 || good.tag == 4)
                p.writeC(4);
            else
              p.writeC((byte)good.tag);
        }
        private static void WriteMatchData(GoodItem good, SendGPacket p)
        {
            p.writeD(good.id);
            p.writeD(good._item._id);
            p.writeD(good._item._count);
        }
        public static bool IsBlocked(string txt, List<int> items)
        {
            lock (ShopUniqueList)
            {
                for (int i = 0; i < ShopUniqueList.Values.Count; i++)
                {
                    GoodItem good = ShopUniqueList.Values[i];

                    if (!items.Contains(good._item._id) && good._item._name.Contains(txt))//conta dentro do goods os valores escritos em rules
                        items.Add(good._item._id);//adiciona os ids correspodentes a lista itens
                }
            }
            return false;
        }
        public static GoodItem GetGood(int goodId)
        {
            if (goodId == 0)
                return null;
            lock (ShopAllList)
            {
                for (int i = 0; i < ShopAllList.Count; i++)
                {
                    GoodItem good = ShopAllList[i];
                    if (good.id == goodId)
                        return good;
                }
            }
            return null;
        }
        public static List<GoodItem> GetGoods(List<CartGoods> ShopCart, out int GoldPrice, out int CashPrice)
        {
            GoldPrice = 0;
            CashPrice = 0;
            List<GoodItem> items = new List<GoodItem>();
            if (ShopCart.Count == 0)
                return items;
            lock (ShopBuyableList)
            {
                for (int i = 0; i < ShopBuyableList.Count; i++)
                {
                    GoodItem good = ShopBuyableList[i];
                    for (int i2 = 0; i2 < ShopCart.Count; i2++)
                    {
                        CartGoods CartGood = ShopCart[i2];
                        if (CartGood.GoodId == good.id)
                        {
                            items.Add(good);
                            switch (CartGood.BuyType)
                            {
                                case 1: GoldPrice += good.price_gold; break;
                                case 2: CashPrice += good.price_cash; break;
                            }
                        }
                    }
                }
            }
            return items;
        }
    }
    public class CartGoods
    {
        public int GoodId, BuyType;
    }
    public class ShopData
    {
        public byte[] Buffer;
        public int ItemsCount, Offset;
    }
}