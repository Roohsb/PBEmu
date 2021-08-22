using Core;
using Core.managers;
using Core.xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Game.data.managers
{
    public static class ClassicModeManager
    {
        public static List<int> itemscamp = new List<int>();
        public static List<int> itemsCupom = new List<int>();

        public static void Parse(string path)
        {
            if (!File.Exists(path))
            {
                SendDebug.SendInfo("[ClassicModeManager] There is no file: " + PathJSON.PathClassiMode);
                return;
            }
            using StreamReader r = new StreamReader(path);
            string json = r.ReadToEnd();
            var data = (JObject)JsonConvert.DeserializeObject(json);
            foreach (JToken article in data["classic"].Children())
            {
                string tournament = article["tournament"].Value<string>();
                string filter = article["name"].Value<string>();
                if (tournament == "camp" && Settings.EnableClassicRules)
                        ShopManager.IsBlocked(filter, itemscamp);
            }
            if (itemscamp.Count > 0)
                Logger.GameSystem($" [System] @Camp: '{itemscamp.Count}'");
        }
        public static bool Clear()
        {
            if (itemscamp.Count > 0)
            {
                itemscamp.Clear();
                return true;
            }
            return false;
        }
        public static void LoadList()
        {
            Parse(PathJSON.PathClassiMode);
        }
        public static bool IsBlocked(int listid, int id) => listid == id;
        public static bool IsBlocked(int listid, int id, ref List<string> list, string category)
        {
            if (listid == id)
            {
                list.Add(category);
                return true;
            }
            return false;
        }
        public static bool IsCupomEffect(int id)
        {
            return true;
        }
    }
}