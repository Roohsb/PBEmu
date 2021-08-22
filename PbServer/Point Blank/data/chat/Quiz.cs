using Core.managers;
using Core.managers.events;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public class Quiz
    {
        public static void EventoQuiz(Account player, string text, Room room)
        {
            var respota = EventoAcerteGanhe.total;
            Channel channel = player.GetChannel();
            if (channel == null)
                return;
            if (player.chancesQuiz < 6)
            {
                player.chancesQuiz += 1;
                if (text == string.Concat(respota))
                {
                    string strAcertou = $"Congratulations you won the quiz, your answer was [{respota}], and won 10,000 cash.";
                    if (room != null)
                        player.SendPacket(new ROOM_CHATTING_PAK(1, room._slots[player._slotId]._id, player.UseChatGM(), strAcertou));
                    else
                        player.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, strAcertou));
                    using (SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK($"[Quiz] Quiz winner was {player.player_name}. answer [{respota}]"))
                        GameManager.SendPacketToAllClients(packet);
                    GameManager.EventsReloadsFalse();
                    player._money += 10000;
                    player.SendPacket(new AUTH_WEB_CASH_PAK(0, player._gp, player._money));
                    PlayerManager.UpdateAccountCash(player.player_id, player._money);
                    EventoAcerteGanhe.total = 0;
                }
                else
                {
                    string strErrou = "Incorrect answer, try again.";
                    if (room != null)
                        player.SendPacket(new ROOM_CHATTING_PAK(1, room._slots[player._slotId]._id, player.UseChatGM(), strErrou));
                    else
                        player.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, strErrou));
                }
            }
            else
            {
                string strLimitado = $"Your attempt limit has been exhausted [{player.chancesQuiz}]";
                if (room != null)
                    player.SendPacket(new ROOM_CHATTING_PAK(1, room._slots[player._slotId]._id, player.UseChatGM(), strLimitado));
                else
                    player.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, player.GetSessionId(), 0, true, strLimitado));
                player.quiz = false;
            }
        }
        public static void ChatTextoEvento(Account player, string texto, ref bool warning)
        {
            if(player.ModelEventChat != null && texto == player.ModelEventChat.str)
            {
                int item1 = player.ModelEventChat.item_id_1;
                int item2 = player.ModelEventChat.item_id_2;
                int dias1 = player.ModelEventChat.count_1;
                int dias2 = player.ModelEventChat.count_2;
                player.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, player, new ItemsModel(item1, ComDiv.GetItemCategory(item1), "Chat Event", 2, uint.Parse(DateTime.Now.AddDays(dias1).ToString("yyMMddHHmm")))), false);
                player.SendPacket(new INVENTORY_ITEM_CREATE_PAK(1, player, new ItemsModel(item2, ComDiv.GetItemCategory(item2), "Chat Event", 2, uint.Parse(DateTime.Now.AddDays(dias2).ToString("yyMMddHHmm")))), false);
                player.SendPacket(new SERVER_MESSAGE_ITEM_RECEIVE_PAK(0), false);
                player.EventChat = PlayerManager.InsertEventChat(player.player_id, true);
                warning = true;
            }
            else
                warning = false;
        }
    }
}
