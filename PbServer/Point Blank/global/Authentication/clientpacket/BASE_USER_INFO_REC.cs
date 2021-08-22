using System;

namespace Game.global.Authentication
{
    public class BASE_USER_INFO_REC : ReceiveGamePacket
    {
        public BASE_USER_INFO_REC(GameClient client, byte[] data)
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
                _client.SendPacket(new BASE_USER_INFO_PAK(_client._player));
            }
            catch(Exception ex)
            {
                SendDebug.SendInfo("BASE_USER_INFO_REC: " + ex.ToString());
            }

        }
    }
}