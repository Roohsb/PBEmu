using Core;
using Core.models.enums;
using Core.models.enums.global;
using System;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;

namespace Game
{
    public static class LicenseSystem
    {
        public static string KeyUser;
        public static int UserID;

        public static void Load()
        {
            try
            {
                ConfigFile configFile = new ConfigFile("config/LorenStudio.ini");
                //
                KeyUser = configFile.ReadString("KeyUser", "0000-0000-0000-0000-0000");
                UserID = configFile.ReadInt32("UserID", 0);
                LicenseKey();
                UpdateKeyLicense();
                VerificGroupMember();
                VerificProductID();
            }
            catch (Exception ex)
            {
               // SendDebug.SendInfo("Settings: " + ex.ToString());
                return;
            }
        }
        public static bool LicenseKey()
        {
            //verifica a KEY e o ID do cliente
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `nexus_licensekeys` WHERE `lkey_key` = @keyUser and `lkey_member` = @UserID and `lkey_active` = 1", db.GetConnection());
            command.Parameters.AddWithValue("@keyUser", KeyUser);
            command.Parameters.AddWithValue("@UserID", UserID);

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show("Key e ID Configurados com sucesso!!");
                VerificKeyActive();
                UpdateCertificado();
                return true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Sua licença expirou entre em contato com o nosso suporte em lorenstudio.com");
               UpdateClosePB();
                Environment.Exit(0);
                return false;
            }
            db.CloseConnection();
        }

        public static bool VerificProductID()
        {
            //verifica o NOME do produto com o USERID do cliente
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `nexus_purchases` WHERE `ps_member` = @UserID and `ps_name` = @ProductName and `ps_active` = 1", db.GetConnection());
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@ProductName", "PBEMU");

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                UpdateCertificado();
                return true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Você está tentando usar uma key de outro produto.");
               UpdateClosePB();
                Environment.Exit(0);
                return false;
            }
            db.CloseConnection();
        }
        public static bool VerificGroupMember()
        {
            //verifica o ID do cliente e se ele esta dentro do grupo que tem permissão para baixar os updates.
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `core_members` WHERE `member_id` = @UserID and `member_group_id` = @ProductName", db.GetConnection());
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@ProductName", "7");

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {

                return true;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Parece que existe um problema com sua licença verifique com o nosso suporte para que seja resolvido o mais rapido possivel!!");
                UpdateClosePB();
                Environment.Exit(0);
                return false;
            }
            db.CloseConnection();
        }

        public static bool UpdateKeyLicense()
        {
            //verifica o ID do cliente e se ele esta dentro do grupo que tem permissão para baixar os updates.
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("INSERT INTO acess_permition (lkey_member, lkey_key, lkey_active) VALUES  (@UserID, @keyUser, 0) ON DUPLICATE KEY UPDATE lkey_member = @UserID", db.GetConnection());
            command.Parameters.AddWithValue("@keyUser", KeyUser);
            command.Parameters.AddWithValue("@UserID", UserID);

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                UpdateCertificado();
                return true;
            }
            else
            {
                UpdateCertificado();
                return false;
            }
            db.CloseConnection();
        }

        public static bool UpdateCertificado()
        {
           
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("UPDATE  acess_permition SET lkey_member = @UserID, lkey_key = @keyUser, lkey_active = 1", db.GetConnection());
            command.Parameters.AddWithValue("@keyUser", KeyUser);
            command.Parameters.AddWithValue("@UserID", UserID);

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
            db.CloseConnection();
        }

        public static bool UpdateClosePB()
        {
            
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("UPDATE  acess_permition SET lkey_member = @UserID, lkey_key = @keyUser, lkey_active = 0", db.GetConnection());
            command.Parameters.AddWithValue("@keyUser", KeyUser);
            command.Parameters.AddWithValue("@UserID", UserID);

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                
                Environment.Exit(0);
                return false;
            }
            db.CloseConnection();
        }

        public static bool VerificKeyActive()
        {
            DB db = new DB();
            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `acess_permition` WHERE `lkey_key` = @keyUser and `lkey_member` = @UserID and `lkey_active` = @KeyActive", db.GetConnection());
            command.Parameters.AddWithValue("@keyUser", KeyUser);
            command.Parameters.AddWithValue("@UserID", UserID);
            command.Parameters.AddWithValue("@KeyActive", "1");

            db.OpenConnection();
            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                System.Windows.Forms.MessageBox.Show("Já existe uma conexão aberta com o seu servidor.");
                UpdateClosePB();
                // Environment.Exit(0);
                return true;
            }
            else
            {
             
                return false;
            }
            db.CloseConnection();
        }

    }
}


