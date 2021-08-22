using Core;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_MISSION_SUCCESS_REC : ReceiveGamePacket
    {
        public BASE_MISSION_SUCCESS_REC(GameClient client, byte[] data)
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