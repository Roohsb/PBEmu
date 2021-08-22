using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Battle.data.xml
{
    public class CharaJSON
    {
        public static List<CharaModel> _charas = new List<CharaModel>();
        public static int getLifeById(int charaId, int type)
        {
            for (int i = 0; i < _charas.Count; i++)
            {
                CharaModel chara = _charas[i];
                if (chara.Id == charaId && chara.Type == type)
                    return chara.Life;
            }
            return 100;
        }
        public static void Load()
        {
            string path = "data/battle/charas.json";
            if (File.Exists(path))
                parse(path);
            else
                Logger.Warning("[CharaXML] Não há nenhum arquivo: " + path);
        }
        private static void parse(string path)
        {
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                foreach (JToken article in data["Chara"].Children())
                {
                    _charas.Add(new CharaModel
                    {
                        Id = int.Parse(article["Id"].Value<string>()),
                        Type = int.Parse(article["Type"].Value<string>()),
                        Life = int.Parse(article["Life"].Value<string>())
                    });
                }
            }
            Logger.Carregar(" [JSON system] Foram Carregados " + _charas.Count + " Charas");
        }
    }
    public class CharaModel
    {
        public int Id, Life, Type;
    }
}