using Core.models.account.rank;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
namespace Core.xml
{
    public class ClanRankJSON
    {
        private static List<RankModel> _ranks = new List<RankModel>();
        public static void Load()
        {
            Parse(PathJSON.PathClanRank);
        }
        public static RankModel GetRank(int rankId)
        {
            lock (_ranks)
            {
                for (int i = 0; i < _ranks.Count; i++)
                {
                    RankModel rank = _ranks[i];
                    if (rank._id == rankId)
                        return rank;
                }
                return null;
            }
        }
        private static void Parse(string path)// XML
        {
            if (!File.Exists(path))
            {
                Logger.Error("[ClanRankXML] Não existe o arquivo: " + PathJSON.PathClanRank);
                return;
            }
            using (StreamReader r = new StreamReader(path))
            {
               var data = (JObject)JsonConvert.DeserializeObject(r.ReadToEnd());
                foreach(JToken article in data["basic"].Children())
                {
                    _ranks.Add(new RankModel(int.Parse(article["id"].Value<string>()),
                        int.Parse(article["onNextLevel"].Value<string>()), 0,
                          int.Parse(article["onAllExp"].Value<string>())));
                }
            }
        }
    }
}