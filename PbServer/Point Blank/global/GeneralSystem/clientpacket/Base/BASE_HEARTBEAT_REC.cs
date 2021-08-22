using Core;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_HEARTBEAT_REC : ReceiveGamePacket
    {
        public BASE_HEARTBEAT_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
        }
    }
}