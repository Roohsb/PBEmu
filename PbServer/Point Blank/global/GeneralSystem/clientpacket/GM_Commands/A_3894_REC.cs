using Core;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class A_3894_REC : ReceiveGamePacket
    {
        private int Slot;
        public A_3894_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            Slot = ReadC();
        }

        public override void Run()
        {
            if (_client == null || _client._player == null)
                return;
            try
            {
                //Ativa quando usa "/EXIT (SLOT)"
                SendDebug.SendInfo("[3894] Slot: " + Slot);
            }
            catch
            {
            }
        }
    }
}