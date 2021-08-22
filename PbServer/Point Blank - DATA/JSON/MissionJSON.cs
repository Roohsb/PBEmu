using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Core.xml
{
    public class MissionJSON
    {
        public static uint _missionPage1, _missionPage2;
        private static List<MissionModel> Missions = new List<MissionModel>();
        public static void Parse(string PathMission)
        {
            if (!File.Exists(PathMission))
            {
                Logger.Error("[PathMission] Não existe o arquivo: " + PathJSON.PathClanRank);
                return;
            }
            try
            {
                using (StreamReader r = new StreamReader(PathMission))
                {
                    var data = (JObject)JsonConvert.DeserializeObject(r.ReadToEnd());
                    foreach (JToken article in data["mission"].Children())
                    {
                        bool enable = bool.Parse(article["Enable"].Value<string>());
                        MissionModel mission = new MissionModel
                        {
                            id = int.Parse(article["MissionID"].Value<string>()),
                            price = int.Parse(article["price"].Value<string>()),
                        };
                        uint flag = (uint)(1 << mission.id);
                        int listId = (int)(Math.Ceiling(mission.id / 32.0));
                        if (enable)
                        {
                            switch (listId)
                            {
                                case 1: _missionPage1 += flag; break;
                                case 2: _missionPage2 += flag; break;
                            }
                        }
                        Missions.Add(mission);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }
        public static void Load()
        {
            Parse(PathJSON.PathMission);
        }
        public static int GetMissionPrice(int id)
        {
            for (int i = 0; i < Missions.Count; i++)
            {
                MissionModel mission = Missions[i];
                if (mission.id == id)
                    return mission.price;
            }
            return -1;
        }
    }
    public class MissionModel
    {
        public int id, price;
    }
}