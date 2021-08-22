using Game.data.model;
using System;

namespace Game.global.Authentication
{
    public class BASE_USER_INVENTORY_REC : ReceiveGamePacket
    {
        public Account p;
        public BASE_USER_INVENTORY_REC(GameClient client, byte[] data)
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
                Account p = _client._player;
                if (p == null)
                    return;
                _client.SendPacket(new BASE_USER_INVENTORY_PAK(p._inventory._items));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BASE_INVENTORY_REC] " + ex.ToString());
            }
        }
    }
}