
using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class A_3890_REC : ReceiveGamePacket
    {
        private int Slot;
        public A_3890_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            Slot = ReadC();
        }

        public override void Run()
        {
            if (_client == null)
                return;
            Account p = _client._player;
            if (p == null || p._slotId == Slot)
                return;
            if (!p.IsGM())
            {
                _client.Close(0, false);
                return;
            }
            try
            {
                Room room = p._room;
                if (room == null)
                    return;
                Account pR = room.GetPlayerBySlot(Slot);
                if (pR == null)
                    return;
                pR.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2));
                pR.Close(1000, true);
                //Ativa quando usa "/KICK (slotid)"
                SendDebug.SendInfo("[3890] Slot: " + Slot);
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo(ex.ToString());
            }
        }
    }
}