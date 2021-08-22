using Core.models.enums.flags;
using Core.xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace Core.managers
{
    public static class CupomEffectManagerJSON
    {
        private static List<CupomFlag> Effects = new List<CupomFlag>();
        public static void Parse(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Error("Error reading EfferManager information");
                return;
            }
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                foreach (JToken article in data["basic"].Children())
                {
                    Effects.Add(new CupomFlag
                    {
                        EffectFlag =(CupomEffects) int.Parse(article["Flag"].Value<string>()),
                        ItemId = int.Parse(article["ItemID"].Value<string>())
                    });
                }
            }
        }

        public static void LoadCupomFlags()
        {
            Parse(PathJSON.PathCupom);
        }
        public static CupomFlag GetCupomEffect(int id)
        {
            for (int i = 0; i < Effects.Count; i++)
            {
                CupomFlag flag = Effects[i];
                if (flag.ItemId == id)
                    return flag;
            }
            return null;
        }
    }

    public class CupomFlag
    {
        public int ItemId;
        public CupomEffects EffectFlag;
    }
}