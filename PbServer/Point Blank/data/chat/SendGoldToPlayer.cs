using Core;
using Core.managers;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class SendGoldToPlayer
    {
        public static string SendByNick(string str) => 
            BaseGiveGold(AccountManager.GetAccount(str.Substring(3), 1, 0));
        public static string SendById(string str) => 
            BaseGiveGold(AccountManager.GetAccount(long.Parse(str.Substring(4)), 0));
        public static string SendById3(string str) 
        {
            string[] gp = str.Substring(4).Split(' ');
            long player_id = long.Parse(gp[0]);
            int gold = int.Parse(gp[1]);
            return BaseGiveGoldAdd(AccountManager.GetAccount(player_id, 0), gold);
        }
        private static string BaseGiveGold(Account pR)
        {
            if (pR == null)
                return Translation.GetLabel("GiveGoldFail");
            if (PlayerManager.UpdateAccountGold(pR.player_id, pR._gp + 10000))
            {
                pR._gp += 10000;
                pR.SendPacket(new AUTH_WEB_CASH_PAK(0, pR._gp, pR._money), false);
                SEND_ITEM_INFO.LoadGoldCash(pR);
                return Translation.GetLabel("GiveGoldSuccess", pR.player_name);
            }
            else
                return Translation.GetLabel("GiveGoldFail2");
        }
        private static string BaseGiveGoldAdd(Account pR, int gold)
        {
            if (pR == null)
                return Translation.GetLabel("GiveGoldFail");
            if (gold < 0)
                return "Gold nao pode ser inferior a 0!";
            else if (gold > 99999999)
                return "gold muito alto.";
            if (PlayerManager.UpdateAccountGold(pR.player_id, pR._gp + gold))
            {
                pR._gp += gold;
                pR.SendPacket(new AUTH_WEB_CASH_PAK(0, pR._gp, pR._money), false);
                SEND_ITEM_INFO.LoadGoldCash(pR);
                return "Você deu [" + gold + "] gold para " + pR.player_name;
            }
            else
                return Translation.GetLabel("GiveGoldFail2");
        }
    }
}