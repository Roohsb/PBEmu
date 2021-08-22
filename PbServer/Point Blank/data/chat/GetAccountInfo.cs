using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Game.data.chat
{
    public static class GetAccountInfo
    {
        public static string getById(string str, Account player) => BaseCode(AccountManager.GetAccount(long.Parse(str.Substring(5)), 2), player);
        public static string getByNick(string str, Account player) => BaseCode(AccountManager.GetAccount(str.Substring(5), 1, 2), player);
        private static string BaseCode(Account p, Account player)
        {
            if (p == null || player == null)
                return Translation.GetLabel("GI_Fail");
            DateTime LastLogin;
            if (p.LastLoginDate == 0)
                LastLogin = new DateTime();
            else
                LastLogin = DateTime.ParseExact(p.LastLoginDate.ToString(), "yyMMddHHmm", CultureInfo.InvariantCulture);

            string info = Translation.GetLabel("GI_Title");
            info += "\n" + Translation.GetLabel("GI_Id", p.player_id);
            info += "\n" + Translation.GetLabel("GI_Nick", p.player_name);
            info += "\n" + Translation.GetLabel("GI_Rank", p._rank);
            info += "\n" + Translation.GetLabel("GI_Fights", p._statistic.fights, p._statistic.fights_win, p._statistic.fights_lost, p._statistic.fights_draw);
            info += "\n" + Translation.GetLabel("GI_KD", p._statistic.GetKDRatio());
            info += "\n" + Translation.GetLabel("GI_HS", p._statistic.GetHSRatio());
            info += "\n" + Translation.GetLabel("GI_LastLogin", (LastLogin == new DateTime() ? "Nunca" : LastLogin.ToString("dd/MM/yy HH:mm")));
            info += "\n" + Translation.GetLabel("GI_LastIP", ((int)player.access >= 5 ? p.PublicIP.ToString() : Translation.GetLabel("GI_BlockedInfo")));
            info += "\n" + Translation.GetLabel("GI_BanObj", player.ban_obj_id);
            if ((int)player.access >= 5)
                info += "\n" + Translation.GetLabel("GI_HaveAccess2", p.access);
            else
                info += "\n" + Translation.GetLabel("GI_HaveAccess1", (p.HaveGMLevel() ? Translation.GetLabel("GI_Yes") : Translation.GetLabel("GI_No")));
            info += "\n" + Translation.GetLabel("GI_UsingFake", (p._bonus != null && p._bonus.fakeRank != 55 ? Translation.GetLabel("GI_Yes") : Translation.GetLabel("GI_No")));
            info += "\n" + Translation.GetLabel("GI_Channel", (p.channelId >= 0 ? string.Format("{0:0##}", (p.channelId + 1)) : Translation.GetLabel("GI_None1")));
            info += "\n" + Translation.GetLabel("GI_Room", (p._room != null ? string.Format("{0:0##}", (p._room._roomId + 1)) : Translation.GetLabel("GI_None2")));
            player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(info));
            return Translation.GetLabel("GI_Success");
        }
        public static string getByIPAddress(string str, Account player)
        {
            List<string> accs = AccountManager.GetAccountsByIP(str.Substring(6));
            string acc = Translation.GetLabel("AccIP_Title");
            for (int i = 0; i < accs.Count; i++)
            {
                acc += "\n" + accs[i];
            }

            player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(acc));
            return Translation.GetLabel("AccIP_Result", accs.Count);
        }
        public static string GetInfoSlot(Room room, string str)
        {
            try
            {
                if (room == null)
                    return "faça isso em uma sala.";
                int slot = int.Parse(str.Substring(6));
                if (slot > -1 && slot < 16)
                {
                    Account p = room.GetPlayerBySlot(slot);
                    if (p != null && p._isOnline)
                    {
                        if (p.HaveAcessLevel())
                            return "Jogador tem acesso!";
                        Channel ch = p.GetChannel();
                        if (ch == null)
                            return "Canal não especificado.";
                        string texto = "Jogador tem o id de: " + p.player_id + " para nick: "+p.player_name+", server: " + ch.serverId + " canal:" + ch._id + " sala: " + p._room._roomId + "";
                        return texto;
                    }
                    else
                    {
                        return "Jogador não existe, ou não está na sala.";
                    }
                }
                else
                {
                    return "Error na informação do slot.";
                }
            }
            catch
            {
                return "Error encontrado, tente novamente.";
            }
        }
    }
}