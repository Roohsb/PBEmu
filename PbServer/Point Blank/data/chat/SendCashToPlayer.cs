using Core;
using Core.managers;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class SendCashToPlayer
    {
        public static string SendByNick(string str) => BaseGiveCash(AccountManager.GetAccount(str.Substring(3), 1, 0));
        public static string SendById(string str) => BaseGiveCash(AccountManager.GetAccount(long.Parse(str.Substring(4)), 0));
        public static string SendById3(string str) 
        {
            string[] cp = str.Substring(4).Split(' ');
            long player_id = long.Parse(cp[0]);
            int cash = int.Parse(cp[1]);
           return BaseGiveCash3(AccountManager.GetAccount(player_id, 0), cash); 
        }
        private static string BaseGiveCash(Account pR)
        {
            if (pR == null)
                return Translation.GetLabel("GiveCashFail");
            if (PlayerManager.UpdateAccountCash(pR.player_id, pR._money + 10000))
            {
                pR._money += 10000;
                pR.SendPacket(new AUTH_WEB_CASH_PAK(0, pR._gp, pR._money), false);
                SEND_ITEM_INFO.LoadGoldCash(pR);
                return Translation.GetLabel("GiveCashSuccess", pR.player_name);
            }
            else
                return Translation.GetLabel("GiveCashFail2");
        }
        private static string BaseGiveCash3(Account pR, int cash)
        {
            if (pR == null)
                return Translation.GetLabel("GiveCashFail");
            if (cash < 0)
                return "O dinheiro não pode ser inferior a 0!";
            else if (cash > 99999999)
                return "Muito cash.";
            if (PlayerManager.UpdateAccountCash(pR.player_id, pR._money + cash))
            {
                pR._money += cash;
                pR.SendPacket(new AUTH_WEB_CASH_PAK(0, pR._gp, pR._money), false);
                SEND_ITEM_INFO.LoadGoldCash(pR);
                return "Você deu [" + cash + "] cash para " + pR.player_name;
            }
            else
                return Translation.GetLabel("GiveCashFail2");
        }
    }
}