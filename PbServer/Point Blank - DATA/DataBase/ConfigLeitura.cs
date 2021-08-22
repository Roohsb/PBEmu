namespace Core.DB_Battle
{
    public class ConfigLeitura
    {
        public static ConfigFile configFile = new ConfigFile("config/DataBase.ini");
        public static string dbName, dbHost, dbUser, dbPass;
        //public static int dbPort;
        public static void Loader()
        {
            dbHost = configFile.ReadString("dbhost", "localhost");
            dbName = configFile.ReadString("dbname", "");
            dbUser = configFile.ReadString("dbuser", "root");
            dbPass = configFile.ReadString("dbpass", "");
           // dbPort = configFile.ReadInt32("dbport", 0);
        }
    }
}
