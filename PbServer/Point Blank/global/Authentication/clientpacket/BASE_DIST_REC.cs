using Core;
using System;

namespace Game.global.Authentication
{
    public class BASE_DIST_REC : ReceiveGamePacket
    {
        public BASE_DIST_REC(GameClient lc, byte[] buff)
        {
            Inicial(lc, buff);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                _client.SendPacket(new BASE_DIST_PAK());

            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
    }
}