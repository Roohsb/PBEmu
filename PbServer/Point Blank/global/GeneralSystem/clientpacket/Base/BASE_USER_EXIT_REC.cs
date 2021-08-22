using Core;
using Game.global.Authentication;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_USER_EXIT_REC : ReceiveGamePacket
    {
        public BASE_USER_EXIT_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }
        public override void Read()
        {
        }
        public override void Run()
        {
            try
            {
                if(LoginManager.Config.ExitURL != null && Settings.EnableTimeReal)
                    _client.SendPacket(new BASE_EXIT_URL_PAK(LoginManager.Config.ExitURL));
                _client.SendPacket(new BASE_USER_EXIT_PAK());
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
            _client.Close(1000);
        }
    }
}