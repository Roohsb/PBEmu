using System.Data.SqlClient;
using System.Runtime.Remoting.Contexts;

namespace Core.DB_Battle
{
    [Synchronization]
    public class ServerLoadDB
    {

        protected SqlConnectionStringBuilder connBuilder;

        public ServerLoadDB()
        {
            connBuilder = new SqlConnectionStringBuilder
            {
                InitialCatalog = ConfigLeitura.dbName,
                DataSource = ConfigLeitura.dbHost,
                UserID = ConfigLeitura.dbUser,
                Password = ConfigLeitura.dbPass
            };
        }

        public static ServerLoadDB GetInstance() => new ServerLoadDB();

        public SqlConnection Conn() => new SqlConnection(connBuilder.ConnectionString);
    }
}
