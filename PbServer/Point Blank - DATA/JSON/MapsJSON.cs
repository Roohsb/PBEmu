using Core.models.room;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;


namespace Core.xml
{
    public class MapsJSON
    {
        public static List<byte> TagList = new List<byte>();
        public static List<ushort> ModeList = new List<ushort>();
        public static ConcurrentDictionary<int, Map> maps = new ConcurrentDictionary<int, Map>();
        public static uint maps1, maps2, maps3, maps4;
        public static void Load()
        {
          //  Parse(PathJSON.pathMap);
        }
        private static void Parse(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Logger.Error("[MapsXML] There is no file: " + path);
                    return;
                }
                using (StreamReader r = new StreamReader(path))
                {
                    int idx = 0, list2 = 1, mapaid = 0;
                    var data = (JObject)JsonConvert.DeserializeObject(r.ReadToEnd());
                    foreach (JToken article in data["map"].Children())
                    {
                        uint flag = (uint)(1 << idx++);
                        int list = list2;
                        if (idx == 32) { list2++; idx = 0; }
                        TagList.Add(byte.Parse(article["tag"].Value<string>()));
                        ModeList.Add(ushort.Parse(article["mode"].Value<string>()));
                        bool enable = bool.Parse(article["enable"].Value<string>());
                        if (!enable)
                            continue;

                        ++mapaid;

                        maps.TryAdd(mapaid, new Map
                        {
                            _id = mapaid,
                            act = enable
                        });

                        switch (list)
                        {
                            case 1: maps1 += flag; break;
                            case 2: maps2 += flag; break;
                            case 3: maps3 += flag; break;
                            case 4: maps4 += flag; break;
                            default:
                                Logger.Error("[List not defined] Flag: " + flag + "; List: " + list);
                                break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
        }
    }
}