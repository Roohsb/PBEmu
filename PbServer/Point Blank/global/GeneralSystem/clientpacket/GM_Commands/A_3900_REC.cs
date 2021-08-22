using Core;

namespace Game.global.GeneralSystem.clientpacket
{
    public class A_3900_REC : ReceiveGamePacket
    {
        private int Slot;
        private string Reason;
        public A_3900_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            Slot = ReadC();
            Reason = ReadS(ReadC());
        }

        public override void Run()
        {
            if (_client == null || _client._player == null)
                return;
            try
            {
                //Ativa quando usa "/BLOCK (SLOT) (REASON)"
                SendDebug.SendInfo("[3900] Slot: " + Slot + "; Reason: " + Reason);
            }
            catch
            {
            }
        }
    }
}