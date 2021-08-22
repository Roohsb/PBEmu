using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Game
{
    public class DB
    {
   public MySqlConnection connection = new MySqlConnection("server=167.114.222.56;port=3306;username=lorenstu_luciferxx;password=ghBPUWEsODV~;database=lorenstu_forumstudio");
        // create a function to open the connection
        public void OpenConnection()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("Conexão feita com sucesso!!");
            }
        }

        // create a function to close the connection
        public void CloseConnection()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                connection.Close();
            }
            else
            {
                //System.Windows.Forms.MessageBox.Show("Conexão não foi estabelecida!!");
            }
        }

        // create a function to return the connection
        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}