using Core.DB_Battle;
using Core.server;

using System;
using System.Data.SqlClient;
using System.Data;

namespace Core.managers
{
    public static class BanManager
    {
        public static BanHistory GetAccountBan(long object_id)
        {
            BanHistory ban = new BanHistory();
            if (object_id == 0)
                return ban;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@obj", object_id);
                    command.CommandText = "SELECT * FROM ban_history WHERE object_id=@obj";
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            ban.object_id = object_id;
                            ban.provider_id = data.GetInt64(1);
                            ban.type = data.GetString(2);
                            ban.value = data.GetString(3);
                            ban.reason = data.GetString(4);
                            ban.startDate = data.GetDateTime(5);
                            ban.endDate = data.GetDateTime(6);
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
            return ban;
        }
        public static bool selectbanmac(string mac)
        {
            bool contaisnbann = false;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@mac", mac);
                    command.CommandText = "SELECT * FROM Ban_Mac WHERE value in (@mac)";
                    using (SqlDataReader data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            string ismac = data.GetString(0);
                            if (ismac != null)
                                contaisnbann = true;
                        }
                        if (data != null)
                            data.Close();
                    }
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
                return contaisnbann;
            }
            catch
            {
                return false;
            }
        }
        public static bool banMacAdd(string v)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@mac", v);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "INSERT INTO Ban_Mac(mac)VALUES(@mac)";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void SaveAutoBan(long player_id, string login, string player_name, string type, string time, string ip, string hack_type)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                   SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@player_id", player_id);
                    command.Parameters.AddWithValue("@login", login);
                    command.Parameters.AddWithValue("@player_name", player_name);
                    command.Parameters.AddWithValue("@type", type);
                    command.Parameters.AddWithValue("@time", time);
                    command.Parameters.AddWithValue("@ip", ip);
                    command.Parameters.AddWithValue("@hack_type", hack_type);
                    command.CommandText = "INSERT INTO auto_ban(player_id, login, player_name, type, time, ip, hack_type) VALUES (@player_id, @login, @player_name, @type, @time, @ip, @hack_type)";
                    command.ExecuteNonQuery();
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch
            {

            }
        }

        public static BanHistory SaveHistory(long provider, string type, string value, DateTime end)
        {
            BanHistory ban = new BanHistory()
            {
                provider_id = provider,
                type = type,
                value = value,
                endDate = end
            };
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@provider", ban.provider_id);
                    command.Parameters.AddWithValue("@type", ban.type);
                    command.Parameters.AddWithValue("@value", ban.value);
                    command.Parameters.AddWithValue("@reason", ban.reason);
                    command.Parameters.AddWithValue("@start", ban.startDate);
                    command.Parameters.AddWithValue("@end", ban.endDate);
                    command.CommandText = "INSERT INTO ban_history(provider_id,type,value,reason,start_date,end_date)VALUES(@provider,@type,@value,@reason,@start,@end)" + "SELECT CAST(scope_identity() AS int)";
                    object data = Convert.ToInt32(command.ExecuteScalar());
                    ban.object_id = (long)data;
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                    if (SaveObjid((long)data, int.Parse(value)))
                    {
                        return ban;
                    }
                    return null;
                }
            }
            catch
            { 
                return null;
            }
        }
        public static bool SaveBanReason(long object_id, string reason) =>
            ComDiv.UpdateDB("ban_history", "reason", reason, "object_id", object_id);
        public static bool SaveObjid(long object_ban, long object_id) =>
            ComDiv.UpdateDB("accounts", "ban_obj_id", object_ban, "player_id", object_id);
    }
    public class BanHistory
    {
        public long object_id, provider_id;
        public string type, value, reason;
        public DateTime startDate, endDate;
        public BanHistory()
        {
            startDate = DateTime.Now;
            type = "";
            value = "";
            reason = "";
        }
    }
}