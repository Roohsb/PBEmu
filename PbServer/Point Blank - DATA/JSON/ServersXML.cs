using Core.DB_Battle;
using Core.models.servers;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.xml
{
    public class ServersXML
    {
        public static List<GameServerModel> _servers = new List<GameServerModel>();
        public static GameServerModel getServer(int id)
        {
            lock (_servers)
            {
                for (int i = 0; i < _servers.Count; i++)
                {
                    GameServerModel server = _servers[i];
                    if (server._id == id)
                        return server;
                }
                return null;
            }
        }
        public static void Load()
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandText = "SELECT * FROM info_gameservers ORDER BY id ASC";
                    command.CommandType = CommandType.Text;
                    SqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        _servers.Add(new GameServerModel(data.GetString(3), (ushort)data.GetInt32(5))
                        {
                            _id = data.GetInt32(0),
                            _state = data.GetInt32(1),
                            _type = data.GetInt32(2),
                            _port = (ushort)data.GetInt32(4),
                            _maxPlayers = data.GetInt32(6)
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
        public static void UpdateServer(int serverId)
        {
            GameServerModel server = getServer(serverId);
            if (server == null)
                return;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@id", serverId);
                    command.CommandText = "SELECT * FROM info_gameservers WHERE id=@id";
                    command.CommandType = CommandType.Text;
                    SqlDataReader data = command.ExecuteReader();
                    while (data.Read())
                    {
                        server._state = data.GetInt32(1);
                        server._type = data.GetInt32(2);
                        server._ip = data.GetString(3);
                        server._port = (ushort)data.GetInt32(4);
                        server._syncPort = (ushort)data.GetInt32(5);
                        server._maxPlayers = data.GetInt32(6);
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
    }
}