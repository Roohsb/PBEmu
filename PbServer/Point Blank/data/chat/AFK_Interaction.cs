using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;

namespace Game.data.chat
{
    public static class AFK_Interaction
    {
        public static string GetAFKCount(string str) => 
            Translation.GetLabel("AFK_Count_Success", GameManager.KickCountActiveClient(double.Parse(str.Substring(9))));
        public static string KickAFKPlayers(string str) =>
            Translation.GetLabel("AFK_Kick_Success", GameManager.KickActiveClient(double.Parse(str.Substring(8))));
        public static string Attentionplayer(string str)
        {
            string[] value = str.Substring(10).Split(' ');
            long playerid = long.Parse(value[0]);
            string msg = value[1];
            if(playerid == 0)
                return "o jogador deve existir!";
            if (msg == null)
                return "você precisa digitar a mensagem.";
            Account p = AccountManager.GetAccount(playerid, 0);
            if(p != null)
            {
                if (p.IsGM())
                    return "faça isso em um jogador.";
                p.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(msg));
                return "o jogador recebeu sua mensagem.";
            }
            else
            {
                return "O jogador não existe ou está offline.";
            }
        }
    }
}