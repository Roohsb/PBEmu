using Core;
using Core.managers;
using Core.xml;
using Game.data.managers;
using System;

namespace Game.data.chat
{
    public class SetLoader
    {
        public static string LoaderArchives()
        {
            try
            {
                RankJSON.RankAwards();
                BasicInventoryJSON.Load();
                ClassicModeManager.LoadList();
                CupomEffectManagerJSON.LoadCupomFlags();
                Translation.Clear();
                Translation.Load();
                return "Server Loaded successfully.";
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("Error Read files: " + ex.ToString());
                return "Error: ";
            }
        }
    }
}
