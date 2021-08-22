﻿using Core;
using Core.managers;
using Core.models.enums;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public static class SetAcessToPlayer
    {
        public static string SetAcessPlayer(string str)
        {
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            long player_id = Convert.ToInt64(split[0]);
            int acess = Convert.ToInt32(split[1]);

            Account pR = AccountManager.GetAccount(player_id, 0);
            if (pR == null)
                return Translation.GetLabel("[*]SetAcess_Fail4");
            if (acess < 0 || acess > 6)
                return Translation.GetLabel("[*]SetAcess_Fail4");
            if (PlayerManager.UpdateAccountAcess(pR.player_id, acess))
            {
                try
                {
                    pR.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                    pR.Close(1000, true);
                    return Translation.GetLabel("SetAcessS", acess, pR.player_name);
                }
                catch { return Translation.GetLabel("SetAcessF"); }
            }
            else
                return Translation.GetLabel("SetAcessF");
        }
        public static string SetAcessPlayerString(string str)
        {
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            string playernick = (split[0]);
            int acess = Convert.ToInt32(split[1]);

            Account pR = AccountManager.GetAccount(playernick, 1, 0);
            if (pR == null)
                return Translation.GetLabel("[*]SetAcess_Fail4");
            if (acess < 0 || acess > 6)
                return Translation.GetLabel("[*]SetAcess_Fail4");
            if (PlayerManager.UpdateAccountAcess(pR.player_id, acess))
            {
                try
                {
                    pR.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                    pR.Close(1000, true);
                    return Translation.GetLabel("SetAcessS", 0, pR.player_name);
                }
                catch { return Translation.GetLabel("SetAcessF"); }
            }
            else
                return Translation.GetLabel("SetAcessF");
        }
        public static string SetAcessPlayerTimeRealString(Account pR)
        {
            if (PlayerManager.UpdateAccountAcess(pR.player_id, 6))
            {
                try
                {
                    pR.access = AccessLevel.Developer;
                    return "Você agora é GM.";
                }
                catch { return Translation.GetLabel("SetAcessF"); }
            }
            else
                return Translation.GetLabel("SetAcessF");
        }
    }
}