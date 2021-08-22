using Core;
using Core.managers;
using Core.managers.server;
using Core.models.shop;
using Game.data.model;

namespace Game.data.chat
{
    public static class EnableMissions
    {
        public static string GenCode1(string str, Account player)
        {
            bool activate = bool.Parse(str.Substring(8));
            bool result = ServerConfigSyncer.UpdateMission(LoginManager.Config, activate);
            if (result)
            {
                SendDebug.SendInfo(Translation.GetLabel("ActivateMissionsWarn", activate, player.player_name));
                return Translation.GetLabel("ActivateMissionsMsg1");
            }
            else
                return Translation.GetLabel("ActivateMissionsMsg2");
        }
    }
}