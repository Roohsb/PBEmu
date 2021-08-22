﻿using Core;
using Core.managers;
using Core.models.account;
using Core.models.shop;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public static class SendGoldToPlayerDev
    {
        public static string SendGoldToPlayer(string str)
        {           
            string txt = str.Substring(str.IndexOf(" ") + 1);
            string[] split = txt.Split(' ');
            long player_id = Convert.ToInt64(split[0]);
            int gold = Convert.ToInt32(split[1]);

            Account pR = AccountManager.GetAccount(player_id, 0);
            if (pR == null)
                return Translation.GetLabel("[*]SendCash_Fail4");
            if (pR._gp + gold> 999999999)
                return Translation.GetLabel("[*]SendCash_Fail4");
            if (PlayerManager.UpdateAccountCash(pR.player_id, pR._gp + gold))
            {
                pR._gp += gold;
                pR.SendPacket(new AUTH_WEB_CASH_PAK(0, pR._gp, pR._money), false);
                SEND_ITEM_INFO.LoadGoldCash(pR);
                return Translation.GetLabel("GiveCashSuccessD", pR._gp, pR.player_name);
            }
            else
                return Translation.GetLabel("GiveCashFail2");
        }
    }
}