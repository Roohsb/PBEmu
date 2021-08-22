using Core;
using Core.DB_Battle;
using Core.server;
using Game.data.model;
using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Game.data.xml
{
    public static class ChannelsXML
    {
        public static List<Channel> _channels = new List<Channel>();
        public static void Load(int serverId)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@server", serverId);
                    command.CommandText = "SELECT * FROM info_channels WHERE server_id=@server ORDER BY channel_id ASC";
                    SqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        _channels.Add(new Channel()
                        {
                            serverId = data.GetInt32(0),
                            _id = data.GetInt32(1),
                            _type = data.GetInt32(2),
                            _announce = data.GetString(3)
                        });
                    }
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
        public static Channel getChannel(int id)
        {
            try
            {
                return _channels[id];
            }
            catch { return null; }
        }
        public static bool updateNotice(int serverId, int channelId, string text) => ComDiv.UpdateDB("info_channels", "announce", text, "server_id", serverId, "channel_id", channelId);
        public static bool updateNotice(string text) => ComDiv.UpdateDB("info_channels", "announce", text);
    }
}