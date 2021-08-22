using Core.DB_Battle;
using Core.models.account;
using Core.models.account.players;
using Core.models.account.rank;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Xml;

namespace Core.xml
{
    public class RankJSON
    {
        private static List<RankModel> _ranks = new List<RankModel>();
        private static SortedList<int, List<ItemsModel>> _awards = new SortedList<int, List<ItemsModel>>();
        public static List<RankModule> Generais = new List<RankModule>();
        public static bool show = false;
        public static void Load()
        {
            Parse(PathJSON.PathRank);
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
        public static void GetGenerais()
        {
            try
            {
                RankModule module = null;
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM accounts ORDER BY player_id ASC";
                    command.CommandType = CommandType.Text;
                    SqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        module = new RankModule()
                        {
                            Player_id = data.GetInt32(2),
                            rank = data.GetInt32(6),
                            Player_Exp = data.GetInt32(8),
                        };
                        if(module.rank < 51 && module.rank > 47)
                        {
                            Generais.Add(module);
                            show = true;
                        }
                    }
                    if(show)
                    Logger.Info("   [Sistema] existem [" + Generais.Count.ToString() + "] top Ranking no jogo.");
                    command.Dispose();
                    data.Close();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        private static void Parse(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    Logger.Error("[RankXML] Não existe o arquivo: " + path);
                    return;
                }
                using (StreamReader r = new StreamReader(path))
                {
                    string json = r.ReadToEnd();
                    var data = (JObject)JsonConvert.DeserializeObject(json);
                    foreach (JToken article in data["Rank"].Children())
                    {
                        _ranks.Add(new RankModel(int.Parse(article["id"].Value<string>()),
                            int.Parse(article["onNextLevel"].Value<string>()),
                             int.Parse(article["onGPUp"].Value<string>()),
                                 int.Parse(article["onAllExp"].Value<string>())));

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }
        public static void RankAwards()
        {
            try
            {


            string path = PathJSON.PathRankUp;
            if (!File.Exists(path))
            {
                Logger.Error("[RankAwards] Não existe o arquivo: " + path);
                return;
            }
            using (StreamReader r = new StreamReader(path))
            {
                string json = r.ReadToEnd();
                var data = (JObject)JsonConvert.DeserializeObject(json);
                foreach (JToken article in data["Rank"].Children())
                {
                    int rank = int.Parse(article["rank_id"].Value<string>());
                    AddItemToList(rank, new ItemsModel(int.Parse(article["item_id"].Value<string>()))
                    {
                        _name = article["Name"].Value<string>(),
                        _count = uint.Parse(article["count"].Value<string>()),
                        _equip = int.Parse(article["equip"].Value<string>()),
                    });
                }
            }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public static List<ItemsModel> GetAwards(int rank)
        {
            lock (_awards)
            {
                if (_awards.TryGetValue(rank, out List<ItemsModel>  model))
                    return model;
            }
            return new List<ItemsModel>();
        }
        private static void AddItemToList(int rank, ItemsModel item)
        {
            if (_awards.ContainsKey(rank))
                _awards[rank].Add(item);
            else
            {
                _awards.Add(rank, new List<ItemsModel>
                {
                    item
                });
            }
        }
    }
}