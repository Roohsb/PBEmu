using Core.DB_Battle;
using Core.models.account.title;
using Core.server;

using System;
using System.Data.SqlClient;
using System.Data;

namespace Core.managers
{
    public class TitleManager
    {
        public static TitleManager GetInstance() => new TitleManager();
        public bool CreateTitleDB(long player_id)
        {
            if (player_id == 0)
                return false;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand cmd = connection.CreateCommand();
                    connection.Open();
                    cmd.Parameters.AddWithValue("@owner", player_id);
                    cmd.CommandText = "INSERT INTO player_titles (owner_id) VALUES (@owner)";
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                return false;
            }
        }
        public PlayerTitles GetTitleDB(long pId)
        {
            PlayerTitles title = new PlayerTitles();
            if (pId == 0)
                return title;
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@owner", pId);
                    command.CommandText = "SELECT * FROM player_titles WHERE owner_id=@owner";
                    command.CommandType = CommandType.Text;
                    SqlDataReader data = command.ExecuteReader();
                    {
                        while (data.Read())
                        {
                            title.ownerId = pId;
                            title.Equiped1 = data.GetInt32(1);
                            title.Equiped2 = data.GetInt32(2);
                            title.Equiped3 = data.GetInt32(3);
                            title.Flags = data.GetInt64(4);
                            title.Slots = data.GetInt32(5);
                        }
                        data.Close();
                    }
                    command.Dispose();
                    connection.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Ocorreu um problema ao carregar os títulos!\r\n" + ex.ToString());
            }
            return title;
        }
        public bool UpdateEquipedTitle(long player_id, int index, int titleId) =>
            ComDiv.UpdateDB("player_titles", "titleequiped" + (index + 1), titleId, "owner_id", player_id);
        public void UpdateTitlesFlags(long player_id, long flags)
        {
            ComDiv.UpdateDB("player_titles", "titleflags", flags, "owner_id", player_id);
        }
        public void UpdateRequi(long player_id, int medalhas, int insignias, int ordens_azuis, int broche)
        {
            try
            {
                using (SqlConnection connection = ServerLoadDB.GetInstance().Conn())
                {
                    SqlCommand command = connection.CreateCommand();
                    connection.Open();
                    command.Parameters.AddWithValue("@pid", player_id);
                    command.Parameters.AddWithValue("@broche", broche);
                    command.Parameters.AddWithValue("@insignias", insignias);
                    command.Parameters.AddWithValue("@medalhas", medalhas);
                    command.Parameters.AddWithValue("@ordensazuis", ordens_azuis);
                    command.CommandType = CommandType.Text;
                    command.CommandText = "UPDATE accounts SET brooch=@broche, insignia=@insignias, medal=@medalhas, blue_order=@ordensazuis WHERE player_id=@pid";
                    command.ExecuteNonQuery();
                    command.Dispose();
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