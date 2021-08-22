using Game.data.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Progress
{
  public class Listcache
    {
        public static SortedList<string, DateTime> NextCashON = new SortedList<string, DateTime>(), NetMsg = new SortedList<string, DateTime>();
        public static SortedDictionary<long, DateTime> Chat = new SortedDictionary<long, DateTime>();
        public static List<XChat> xchat = new List<XChat>();
        public static List<string> _conn = new List<string>();
        public static List<long> VipStartVote = new List<long>();
        public static int Salas, pvps;
        public static bool SysBotChannel = false;
    }
}
