using Core;
using Core.models.servers;
using Core.xml;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public static class PlayersCountInServer
    {
        public static string GetMyServerPlayersCount() => Translation.GetLabel("UsersCount", GameManager.PlayersOnlines(), Settings.serverId);
        public static string GetServerPlayersCount(string str)
        {
            int serverId = int.Parse(str.Substring(9));
            GameServerModel server = ServersXML.getServer(serverId);
            if (server != null)
                return Translation.GetLabel("UsersCount2", server._LastCount, server._maxPlayers, serverId);
            else
                return Translation.GetLabel("UsersInvalid");
        }

        public static string GetServerPlayersNicks(Account ac)
        {
            string str = string.Empty;
            int idx = 0;
           foreach(GameClient GC in GameManager._socketList.Values)
            {
                if(GC!= null && GC._client != null)
                {
                    Account pr = GC._player;
                    if(pr != null && (int)pr.access <= 2 && pr._isOnline)
                    {
                        str += $"nick: {pr.player_name} \n";
                        idx += 1;
                    }
                }
            }
            using SERVER_MESSAGE_ANNOUNCE_PAK isNicks = new SERVER_MESSAGE_ANNOUNCE_PAK(str);
            ac.SendPacket(isNicks);
            return "Accounts. ~ online: " + idx;
        }
    }
}