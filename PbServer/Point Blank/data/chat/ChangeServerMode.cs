using Core;

namespace Game.data.chat
{
    public static class ChangeServerMode
    {
        public static string EnableTestMode()
        {
            if (Settings.isTestMode)
                return Translation.GetLabel("AlreadyTestModeOn");
            else
            {
                Settings.isTestMode = true;
                return Translation.GetLabel("TestModeOn");
            }
        }
        public static string EnablePublicMode()
        {
            if (!Settings.isTestMode)
                return Translation.GetLabel("AlreadyTestModeOff");
            else
            {
                Settings.isTestMode = false;
                return Translation.GetLabel("TestModeOff");
            }
        }
    }
}