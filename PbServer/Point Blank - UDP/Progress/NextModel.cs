using System.Collections.Generic;

namespace Battle
{
    public class NextModel
    {
        public static bool corrupetd = false;
        public static List<string> _offset = new List<string>();

        public static void AddOffet(string addr)
        {
            _offset.Add(addr);
        }
        public static bool ContemOffset(string addr) => _offset.Contains(addr);
    }
}
