using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Game.data.xml
{
    public static class BattleServerJSON
    {
        public static List<BattleServer> Servers = new List<BattleServer>();
        public static void Load()
        {
            string path = "data/battle/udplist.json";
            if (File.Exists(path))
                Parse(path);
            else
                Logger.Info("[BattleServerXML] There is no file: " + path);
        }
        public static BattleServer GetRandomServer()
        {
            if (Servers.Count == 0)
                return null;
            try
            {
                return Servers[new Random().Next(Servers.Count)];
            }
            catch { return null; }
        }
        private static void Parse(string path)
        {
            using StreamReader r = new StreamReader(path);
            var data = (JObject)JsonConvert.DeserializeObject(r.ReadToEnd());
            foreach (JToken article in data["server"].Children())
            {
                Servers.Add(new BattleServer(article["ip"].Value<string>(), int.Parse(article["sync"].Value<string>()))
                {
                    Port = int.Parse(article["port"].Value<string>()),
                });
            }
        }
    }
    public class BattleServer
    {
        public string IP;
        public int Port, SyncPort;
        public IPEndPoint Connection;
        public BattleServer(string ip, int syncPort)
        {
            IP = ip;
            SyncPort = syncPort;
            Connection = new IPEndPoint(IPAddress.Parse(ip), syncPort);
        }
    }
}