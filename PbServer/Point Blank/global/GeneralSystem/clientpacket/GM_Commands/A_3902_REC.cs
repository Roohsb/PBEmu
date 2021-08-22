using Core;

namespace Game.global.GeneralSystem.clientpacket
{
    public class A_3902_REC : ReceiveGamePacket
    {
        public A_3902_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            if (_client == null || _client._player == null)
                return;
            try
            {
                //Ativa quando usa "/ROOMDEST"
                SendDebug.SendInfo("[3902]");
            }
            catch
            {
            }
        }
    }
}