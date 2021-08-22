using Core;
using System;

namespace Game.global.Authentication
{
    public class A_2666_REC : ReceiveGamePacket
    {
        public A_2666_REC(GameClient lc, byte[] buff)
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
                _client.SendPacket(new BASE_RANK_AWARDS_PAK());
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
    }
}