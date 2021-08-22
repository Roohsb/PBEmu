﻿using Core.DB_Battle;
using Core.models.account;
using Core.models.enums;
using Core.server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Core.managers
{
    public static class MessageManager
    {
        public static Message GetMessage(int objId, long pId)
        {
            Message msg = null;
            if (pId == 0 || objId == 0)
                return null;
            try
            {
                DateTime today = DateTime.Now;
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@obj", objId);
                    command.Parameters.AddWithValue("@owner", pId);
                    command.CommandText = "SELECT * FROM player_messages WHERE object_id=@obj AND owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            msg = new Message(data.GetInt64(8), today)
                            {
                                object_id = objId,
                                sender_id = data.GetInt64(2),
                                clanId = data.GetInt32(3),
                                sender_name = data.GetString(4),
                                text = data.GetString(5),
                                type = data.GetInt32(6),
                                state = data.GetInt32(7),
                                cB = (NoteMessageClan)data.GetInt32(9)
                            };
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
                return null;
            }
            return msg;
        }
        public static List<Message> GetGifts(long owner_id)
        {
            List<Message> gifts = new List<Message>();
            if (owner_id == 0)
                return gifts;
            try
            {
                DateTime today = DateTime.Now;
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT * FROM player_messages WHERE owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            int type = data.GetInt32(6);
                            if (type != 2)
                                continue;
                            gifts.Add(new Message(data.GetInt64(8), today)
                            {
                                object_id = data.GetInt32(0),
                                sender_id = data.GetInt64(2),
                                clanId = data.GetInt32(3),
                                sender_name = data.GetString(4),
                                text = data.GetString(5),
                                type = type,
                                state = data.GetInt32(7),
                                cB = (NoteMessageClan)data.GetInt32(9),
                            });
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
            return gifts;
        }
        public static List<Message> GetMessages(long owner_id)
        {
            List<Message> msgs = new List<Message>();
            if (owner_id == 0)
                return msgs;
            try
            {
                DateTime today = DateTime.Now;
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT * FROM player_messages WHERE owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            int type = data.GetInt32(6);
                            if (type == 2)
                                continue;
                            msgs.Add(new Message(data.GetInt64(8), today)
                            {
                                object_id = data.GetInt32(0),
                                sender_id = data.GetInt64(2),
                                clanId = data.GetInt32(3),
                                sender_name = data.GetString(4),
                                text = data.GetString(5),
                                type = type,
                                state = data.GetInt32(7),
                                cB = (NoteMessageClan)data.GetInt32(9),
                            });
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
            return msgs;
        }
        public static bool MessageExists(int objId, long owner_id)
        {
            if (owner_id == 0 || objId == 0)
                return false;
            try
            {
                int msgs = 0;
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@obj", objId);
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT COUNT(*) FROM player_messages WHERE object_id=@obj AND owner_id=@owner";
                    msgs = Convert.ToInt32(command.ExecuteScalar());
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return msgs > 0;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return false;
        }
        public static int GetMsgsCount(long owner_id)
        {
            int msgs = 0;
            if (owner_id == 0)
                return msgs;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.CommandText = "SELECT COUNT(*) FROM player_messages WHERE owner_id=@owner";
                    msgs = Convert.ToInt32(command.ExecuteScalar());
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
            return msgs;
        }
        /// <summary>
        /// Cria uma mensagem na Database, e após a criação é adicionado o número do objeto ao modelo.
        /// </summary>
        /// <param name="msg">Mensagem</param>
        /// <returns></returns>
        public static bool CreateMessage(long owner_id, Message msg)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", owner_id);
                    command.Parameters.AddWithValue("@sendid", msg.sender_id);
                    command.Parameters.AddWithValue("@clan", msg.clanId);
                    command.Parameters.AddWithValue("@sendname", msg.sender_name);
                    command.Parameters.AddWithValue("@text", msg.text);
                    command.Parameters.AddWithValue("@type", msg.type);
                    command.Parameters.AddWithValue("@state", msg.state);
                    command.Parameters.AddWithValue("@expire", msg.expireDate);
                    command.Parameters.AddWithValue("@cb", (int)msg.cB);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO player_messages(owner_id,sender_id,clan_id,sender_name,text,type,state,expire,cb)VALUES(@owner,@sendid,@clan,@sendname,@text,@type,@state,@expire,@cb)" + "SELECT CAST(scope_identity() AS int)";
                    msg.object_id = Convert.ToInt32(command.ExecuteScalar());
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                    return true;
                }
            }
            catch { return false; }
        }
        public static void UpdateState(int objId, long owner, int value)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@state", value);
                    command.Parameters.AddWithValue("@owner_id", owner);
                    command.Parameters.AddWithValue("@object_id", objId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE player_messages SET state=@state WHERE object_id=@object_id AND owner_id=@owner_id";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }

        public static void UpdateExpireDate(int objId, long owner, long date)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@expire", date);
                    command.Parameters.AddWithValue("@owner_id", owner);
                    command.Parameters.AddWithValue("@object_id", objId);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE player_messages SET expire=@expire WHERE object_id=@object_id AND owner_id=@owner_id";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }

        }

        public static bool DeleteMessage(int objId, long owner)
        {
            if (owner == 0 || objId == 0)
            {
                return false;
            }
            ComDiv.DeleteDB("player_messages", "object_id", objId, "owner_id", owner);
            return true;
        }
        public static bool DeleteMessages(List<object> objs, long owner)
        {
            if (owner == 0 || objs.Count == 0)
            {
                return false;
            }

            ComDiv.DeleteDB("player_messages", "object_id", objs.ToArray(), "owner_id", owner);
            return true;
        }
        public static void RecicleMessages(long owner_id, List<Message> msgs)
        {
            List<object> objs = new List<object>();
            for (int i = 0; i < msgs.Count; i++)
            {
                Message msg = msgs[i];
                if (msg.DaysRemaining == 0)
                {
                    objs.Add(msg.object_id);
                    msgs.RemoveAt(i--);
                }
            }
            DeleteMessages(objs, owner_id);
        }
    }
}