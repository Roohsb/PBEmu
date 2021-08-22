using Core.models.account.mission;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.xml
{
    public class MissionAwardsJSON
    {
        private static List<MisAwards> _awards = new List<MisAwards>();
        public static void Load()
        {
            Parse(PathJSON.PathMissionAwards);
        }

        private static void Parse(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Error("[MissionAwards] Não existe o arquivo: " + path);
                return;
            }
            using(StreamReader r = new StreamReader(path))
            {
               var data = (JObject) JsonConvert.DeserializeObject(r.ReadToEnd());
                foreach (JToken article in data["mission"].Children())
                {
                    int id = int.Parse(article["id"].Value<string>());
                    int blueOrder = int.Parse(article["blueOrder"].Value<string>());
                    int exp = int.Parse(article["exp"].Value<string>());
                    int gp = int.Parse(article["gp"].Value<string>());
                    _awards.Add(new MisAwards(id, blueOrder, exp, gp));
                }
            }
        }
        public static MisAwards getAward(int mission)
        {
            lock (_awards)
            {
                for (int i = 0; i < _awards.Count; i++)
                {
                    MisAwards mis = _awards[i];
                    if (mis._id == mission)
                        return mis;
                }
                return null;
            }
        }
    }
}