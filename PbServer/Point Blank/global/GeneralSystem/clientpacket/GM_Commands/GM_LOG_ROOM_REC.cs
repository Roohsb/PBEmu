using Game.data.model;
using Game.global.serverpacket;

namespace Game.global.GeneralSystem.clientpacket
{
    public class GM_LOG_ROOM_REC : ReceiveGamePacket
    {
        private int slot;
        public GM_LOG_ROOM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slot = ReadC();
        }

        public override void Run()
        {
            Account p = _client._player, pR;
            if (p == null || !p.IsGM())
                return;
            Room room = p._room;
            if (room != null && room.GetPlayerBySlot(slot, out pR))
                _client.SendPacket(new GM_LOG_ROOM_PAK(pR));
        }
    }
}