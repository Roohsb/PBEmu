using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Battle.data.xml
{
    public class MeleeExceptionsJSON
    {
        public static List<MeleeExcep> _items = new List<MeleeExcep>();
        public static bool Contains(int number)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Number == number)
                    return true;
            }
            return false;
        }
        public static void Load()
        {
            string path = "data/battle/exceptions.json";
            if (File.Exists(path))
                parse(path);
            else
                Logger.Warning("[MeleeExceptionsXML] Não há nenhum arquivo: " + path);
        }
        private static void parse(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                foreach (JToken article in data["weapon"].Children())
                {
                    _items.Add(new MeleeExcep
                    {
                        Number = int.Parse(article["number"].Value<string>()),
                    });
                }
                Logger.Carregar(" [JSON system] Foram Carregados " + _items.Count + " Exceções de faca.");
            }
        }
    }
    public class MeleeExcep
    {
        public int Number;
    }
}