using System;
using System.Collections.Generic;
using System.IO;

namespace Core.filters
{
    public static class NickFilter
    {
        public static List<string> _filter = new List<string>();
        public static void Load()
        {
            if (File.Exists("data/player/filters/nicks.txt"))
            {
                try
                {
                    using (StreamReader file = new StreamReader("data/player/filters/nicks.txt"))
                    {
                        string line;
                        while ((line = file.ReadLine()) != null)
                            _filter.Add(line);
                        file.Close();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("[NickFilter] " + ex.ToString());
                }
            }
            else
                Logger.Error("[Aviso]: The filter file 1 does not exist.");
        }
    }
}