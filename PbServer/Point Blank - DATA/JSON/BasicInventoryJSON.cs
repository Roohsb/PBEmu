using Core.models.account.players;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Core.xml
{
    public class BasicInventoryJSON
    {

        public static List<ItemsModel> basic = new List<ItemsModel>(), creationAwards = new List<ItemsModel>();
        public static void Load()
        {
            Parse(PathJSON.PathBasicInventory);
        }

        private static void Parse(string pathBasicInventory)
        {
            if (!File.Exists(pathBasicInventory))
            {
                Logger.Error("[BasicInventory] There is no file: " + PathJSON.PathClanRank);
                return;
            }
            using (StreamReader r = new StreamReader(pathBasicInventory))
            {
                var data = (JObject)JsonConvert.DeserializeObject(r.ReadToEnd());
                foreach (JToken article in data["basic"].Children())
                {
                    switch (int.Parse(article["Type"].Value<string>()))
                    {
                        case 0:
                            {
                                basic.Add(new ItemsModel(int.Parse(article["ItemID"].Value<string>()))
                                {
                                    _name = article["name"].Value<string>(),
                                    _count = uint.Parse(article["count"].Value<string>()),
                                    _equip = int.Parse(article["equip"].Value<string>()),
                                });
                                break;
                            }
                        case 1:
                            {
                                creationAwards.Add(new ItemsModel(int.Parse(article["ItemID"].Value<string>()))
                                {
                                    _name = article["name"].Value<string>(),
                                    _count = uint.Parse(article["count"].Value<string>()),
                                    _equip = int.Parse(article["equip"].Value<string>()),
                                });
                                break;
                            }
                    }
                }
            }
        }
    }
}