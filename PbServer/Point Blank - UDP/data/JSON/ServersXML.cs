﻿using Battle.data.models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Battle.data.xml
{
    public class ServersXML
    {
        private static List<GameServerModel> _servers = new List<GameServerModel>();
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
                using (SqlConnection connection = SQLjec.getInstance().Conn())
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
    }
}