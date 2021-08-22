using Core;
using Core.models.enums;
using Core.models.room;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using Game.Progress;
using System;

namespace Game.data.chat
{
    public class Chattting
    {
        public static string ChatExcpetion(Account admin,string Texto)
        {
            try
            {
                string[] Partition = Texto.Split(' ');
                long playerid = long.Parse(Partition[0]);
                int minutos = int.Parse(Partition[1]);
                Account player = AccountManager.GetAccount(playerid, true);
                if (minutos < 1 || minutos > 60)
                    return "Minutos de gravação de erros";
                else if (player.IsGM() && player.access > AccessLevel.Streamer)
                    return "você não pode bloquear um jogador com carga.";
                else if (admin.player_id == playerid)
                    return "você não pode bloquear-se";
                if (player != null && player._isOnline)
                {
                    player.isChatDate = DateTime.Now;
                    player.IsChatDateFinish = DateTime.Now.AddMinutes(minutos);
                    if (!Listcache.Chat.ContainsKey(player.player_id))
                        Listcache.Chat.Add(player.player_id, player.isChatDate);
                    else
                        return "jogador já acrescentou.";
                    player.isChatMinute = minutos;
                    player.isChatBanned = true;
                    return $"{player.player_name }foi silenciado por: [{minutos}] Minutes";
                }
                else
                    return "Jogador não existe, ou está offline.";
            }
            catch
            {
                return "um erro detectado. (tente novamente)";
            }
        }
        public static string ChatExcpetionN(Account admin, string Texto)
        {
            try
            {
                string[] Partition = Texto.Split(' ');
                string nick = Partition[0];
                int minutos = int.Parse(Partition[1]);
                Account player =  AccountManager.GetAccount(nick, 1, 0);
                if (minutos <= 0 || minutos > 60)
                    return "Minutos de gravação de erros";
                else if (player.IsGM() && player.access > AccessLevel.Streamer)
                    return "você não pode bloquear um jogador com carga.";
                else if (admin.player_name == nick)
                    return "você não pode bloquear-se";
                if (player != null && player._isOnline)
                {
                    player.isChatDate = DateTime.Now;
                    player.IsChatDateFinish = DateTime.Now.AddMinutes(minutos);
                    if (!Listcache.Chat.ContainsKey(player.player_id))
                        Listcache.Chat.Add(player.player_id, player.isChatDate);
                    else
                        return "jogador já acrescentou.";
                    player.isChatMinute = minutos;
                    player.isChatBanned = true;
                    return $"{player.player_name }foi silenciado por: [{minutos}] Minutes";
                }
                else
                    return "Jogador não existe, ou está offline.";
            }
            catch
            {
                return "um erro detectado. (tente novamente)";
            }
        }
        public static string MuteRoom(Room r)
        {
            try
            {
                r.isRoomMute = !r.isRoomMute;
                return !r.isRoomMute ? "a sala foi liberada para os jogadores." : "a sala foi multada para os jogadores.";
            }
            catch
            {
                return "um erro detectado. (tente novamente)";
            }
        }
    }
}
