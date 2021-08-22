using Core;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_SERVER_CHANGE_REC : ReceiveGamePacket
    {
        public BASE_SERVER_CHANGE_REC(GameClient client, byte[] data)
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
                _client.SendPacket(new BASE_SERVER_CHANGE_PAK(0));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
    }
}