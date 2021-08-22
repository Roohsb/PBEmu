using Core;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_GET_LOBBY_USER_LIST_REC : ReceiveGamePacket
    {
        public ROOM_GET_LOBBY_USER_LIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            Account player = _client._player;
            if (player == null)
                return;
            try
            {
                Channel ch = player.GetChannel();
                if (ch != null)
                    _client.SendPacket(new ROOM_GET_LOBBY_USER_LIST_PAK(ch));
            }
            catch (Exception ex) 
            {
                SendDebug.SendInfo(ex.ToString()); 
            }
        }
    }
}