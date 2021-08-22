using System;

namespace Battle.config
{
    public static class Config
    {
        public static string dbName, dbHost, dbUser, dbPass, hosIp, serverIp, udpVersion, ServerVersion;
       // public static int dbPort;
        public static ushort hosPort, maxDrop, syncPort;
        public static bool  sendFailMsg, useMaxAmmoInDrop, HostLogger;
        public static float plantDuration, defuseDuration;
        public static bool useHitMarker;

        public static void LoadDB()
        {
            ConfigFile configFile = new ConfigFile("config/DataBase.ini");
            dbHost = configFile.readString("dbhost", "localhost");
            dbName = configFile.readString("dbname", "");
            dbUser = configFile.readString("dbuser", "root");
            dbPass = configFile.readString("dbpass", "");
            //dbPort = configFile.readInt32("dbport", 0);
        }
        public static void Load()
        {
            try
            {
                ConfigFile configFile = new ConfigFile("config/battle.ini");
                hosIp = configFile.readString("udpIp", "0.0.0.0");
                serverIp = configFile.readString("serverIp", "0.0.0.0");
                hosPort = configFile.readUInt16("udpPort", 57600);
                sendFailMsg = configFile.readBoolean("sendFailMsg", false);
                maxDrop = configFile.readUInt16("maxDrop", 0);
                syncPort = configFile.readUInt16("syncPort", 0);
                plantDuration = configFile.readFloat("plantDuration", 1.0f);
                defuseDuration = configFile.readFloat("defuseDuration", 1.0f);
                useMaxAmmoInDrop = configFile.readBoolean("useMaxAmmoInDrop", false);
                udpVersion = configFile.readString("UDPVersion", "0.0");
                ServerVersion = configFile.readString("ServerVersion", "");
                HostLogger = configFile.readBoolean("HostLogger", false);
                useHitMarker = configFile.readBoolean("useHitMarker", false);
               // DamageChecker = configFile.readBoolean("DamageChecker", false);
            }
            catch(Exception ex)
            {
                Logger.Error("[Sistema] error: " + ex.ToString());
                return;
            }
        }
    }
}