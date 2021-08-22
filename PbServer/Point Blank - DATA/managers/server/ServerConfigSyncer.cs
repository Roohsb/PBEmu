using Core.DB_Battle;
using Core.server;

using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace Core.managers.server
{
    public static class ServerConfigSyncer
    {
        public static List<int> GuardarID = new List<int>();
        public static ServerConfig GenerateConfig(int configId)
        {
            ServerConfig cfg = null;
            if (configId == 0)
                return cfg;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@cfg", configId);
                    command.CommandText = "SELECT * FROM info_login_configs WHERE config_id=@cfg";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            cfg = new ServerConfig
                            {
                                configId = configId,
                                onlyGM = data.GetBoolean(1),
                                missions = data.GetBoolean(2),
                                UserFileList = data.GetString(3),
                                ClientVersion = data.GetString(4),
                                GiftSystem = data.GetBoolean(5),
                                ExitURL = data.GetString(6)
                            };
                            GuardarID.Add(configId);
                        }
                        if (data != null)
                            data.Close();
                    }
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return cfg;
        }
        public static bool UpdateMission(ServerConfig cfg, bool mission)
        {
            cfg.missions = mission;
            return ComDiv.UpdateDB("info_login_configs", "missions", mission, "config_id", cfg.configId);
        }
    }
    public class ServerConfig
    {
        public int configId;
        public string UserFileList, ClientVersion, ExitURL;
        public bool onlyGM, missions, GiftSystem;
    }
}