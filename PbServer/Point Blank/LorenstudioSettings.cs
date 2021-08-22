using Core;
using Core.models.enums;
using Core.models.enums.global;
using System;
using System.Text;

namespace Game
{
    public static class LorenstudioSettings
    {
              public static string ProjectName;
        public static int Vip1Exp, Vip1Gold, Vip1Cash, Vip2Exp, Vip2Gold, Vip2Cash, Vip5Exp, Vip5Gold, Vip5Cash, Vip6Exp, Vip6Gold, Vip6Cash, MultiExpBot;
        public static int NameColorFree, NameColorVip1, NameColorVip2, NameColorVip5, NameColorVip6;
        public static int WeaponIDVip2, WeaponIDVip5, WeaponIDVip6;
        public static int DurationWeaponVIP2, DurationWeaponVIP5, DurationWeaponVIP6;
        public static string NameWeaponVIP2, NameWeaponVIP5, NameWeaponVIP6;
        public static bool ChatLog;

        public static void Load()
        {
            try
            {
                ConfigFile configFile = new ConfigFile("config/LorenStudio.ini");


                ProjectName = configFile.ReadString("ProjectName", "PointLoren");
                  ChatLog = configFile.ReadBoolean("ChatLog", false);
                NameColorFree = configFile.ReadInt32("CorNameGM", 8);
                //
                Vip1Exp = configFile.ReadInt32("Vip1Exp", 60);
                Vip1Gold = configFile.ReadInt32("Vip1Gold", 40);
                Vip1Cash = configFile.ReadInt32("Vip1Cash", 20);
                NameColorVip1 = configFile.ReadInt32("CorNameVIP1", 3);
                //
                Vip2Exp = configFile.ReadInt32("Vip2Exp", 120);
                Vip2Gold = configFile.ReadInt32("Vip2Gold", 100);
                Vip2Cash = configFile.ReadInt32("Vip2Cash", 40);
                NameColorVip2 = configFile.ReadInt32("CorNameVIP2", 6);
                WeaponIDVip2 = configFile.ReadInt32("WeaponIDVIP2", 1105003003);
                NameWeaponVIP2 = configFile.ReadString("NameWeaponVIP2", "AK_SOPMOD_GRS");
                //
                Vip5Exp = configFile.ReadInt32("Vip5Exp", 180);
                Vip5Gold = configFile.ReadInt32("Vip5Gold", 140);
                Vip5Cash = configFile.ReadInt32("Vip5Cash", 60);
                NameColorVip5 = configFile.ReadInt32("CorNameVIP5", 10);
                WeaponIDVip5 = configFile.ReadInt32("WeaponIDVIP2", 100003099);
                NameWeaponVIP5 = configFile.ReadString("NameWeaponVIP2", "M4A1_Elite");
                //
                Vip6Exp = configFile.ReadInt32("Vip6Exp", 300);
                Vip6Gold = configFile.ReadInt32("Vip6Gold", 200);
                Vip6Cash = configFile.ReadInt32("Vip6Cash", 100);
                NameColorVip6 = configFile.ReadInt32("CorNameVIP6", 7);
                WeaponIDVip6 = configFile.ReadInt32("WeaponIDVIP2", 1105003003);
                NameWeaponVIP6 = configFile.ReadString("NameWeaponVIP2", "Bandana Indonesia");
                //
                MultiExpBot = configFile.ReadInt32("MultiExpBot", 1);

            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("Settings: " + ex.ToString());
                return;
            }
        }
    }
}