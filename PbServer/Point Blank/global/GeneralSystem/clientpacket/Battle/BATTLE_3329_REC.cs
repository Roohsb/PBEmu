using Core;
using Core.models.room;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BATTLE_3329_REC : ReceiveGamePacket
    {
        public BATTLE_3329_REC(GameClient client, byte[] data)
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
                Room room = p._room;
                SendDebug.SendInfo("BATTLE_3329_REC. (PlayerID: " + p.player_id + "; Name: " + p.player_name + "; Room: " + (p._room != null ? p._room._roomId : -1) + "; Channel: " + p.channelId + ")");
                if (room != null)
                {
                    SendDebug.SendInfo("Room3329; BOT: " + room.IsBotMode());
                    SLOT slot = room.GetSlot(p._slotId);
                    if (slot != null)
                    {
                        SendDebug.SendInfo("SLOT Id: " + slot._id + "; State: " + slot.state);
                    }
                }
                _client.SendPacket(new A_3329_PAK());
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}