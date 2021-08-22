using Game.data.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Game.Progress
{
   public class CleanComp
    {
        public static bool AccountClean(Account c, string LocalIP, string PublicIP)
        {
            try
            {
                foreach (Account player in from GameClient client in GameManager._socketList.Values let player = client._player where player != null && player._isOnline select player)
                {
                    if(c.LocalIP == player.LocalIP && PublicIP == player._connection.GetIPAddress() && player._isOnline)
                    {
                        Console.WriteLine("connection with different players.");
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return false;
        }
    }
}
