﻿using Core;
using Core.managers;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public static class SetVipToPlayer
    {
        public static string SetVipPlayer(string str)
        {
            try
            {
                return "Desativado - Vip Somente Manualmente";
                string txt = str.Substring(str.IndexOf(" ") + 1);
                string[] split = txt.Split(' ');
                long player_id = Convert.ToInt64(split[0]);
                int vip = Convert.ToInt32(split[1]);

                Account pR = AccountManager.GetAccount(player_id, 0);
                if (pR == null)
                    return Translation.GetLabel("[*]SetVip_Fail4");
                if (vip < 0 || vip > 2)
                    return Translation.GetLabel("[*]SetVip_Fail4");
                if (PlayerManager.UpdateAccountVip(pR.player_id, vip))
                {
                    try
                    {
                        pR.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                        pR.Close(1000, true);
                        return Translation.GetLabel("SetVipS", vip, pR.player_name);
                    }
                    catch { return Translation.GetLabel("SetVipF"); }
                }
                else
                    return Translation.GetLabel("SetVipF");
            }
            catch
            {
                return "Um erro encontrado ao adicionar vip.";
            }
        }
    }
}