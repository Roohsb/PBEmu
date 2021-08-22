using Core;
using Core.managers;
using Core.models.account;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System.Linq;

namespace Game.data.chat
{
    public static class SendMsgToPlayers
    {
        public static string SendToAll(string str)
        {
            string msg = str.Substring(2);
            int count = 0;
            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(msg))
                count = GameManager.SendPacketToAllClients(packet);
            return Translation.GetLabel("MsgAllClients", count);
        }

        public static string SendToRoom(string str, Room room)
        {
            string msg = str.Substring(3);
            if (room == null)
                return Translation.GetLabel("GeneralRoomInvalid");
            using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(msg))
                room.SendPacketToPlayers(packet);
            return Translation.GetLabel("MsgRoomPlayers");
        }
        public static string SendBoxID(string str)
        {
            string[] txt = str.Substring(5).Split(' ');
            long playerid = long.Parse(txt[0]);
            string text = txt[1];
            Account p = AccountManager.GetAccount(playerid, true);
            if(p != null && p._isOnline)
            {
                Message msg = new Message(15)
                {
                    sender_name = LorenstudioSettings.ProjectName,
                    sender_id = playerid,
                    text = text,
                    state = 1
                };
                p.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(msg), false);
                MessageManager.CreateMessage(playerid, msg);
                return "Message Sent to the player.";
            }
            else
                return "Player does not exist.";
        }
        public static string SendBoxAll(string str, Account account)
        {
            int count = 0;
            Message msg = new Message(15)
            {
                sender_name = LorenstudioSettings.ProjectName,
                sender_id = account.player_id,
                text = str.Substring(5),
                state = 1
            };
            foreach (Account player in from GameClient client in GameManager._socketList.Values let player = client._player where player != null && player._isOnline select player)
            {
                player.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(msg), false);
                MessageManager.CreateMessage(player.player_id, msg);
                count++;
            }
            return "Message sent to everyone. total [" + count+"]"; 
        }
    }
}