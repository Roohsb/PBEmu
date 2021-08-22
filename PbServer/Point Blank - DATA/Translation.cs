using System.Collections.Generic;
using System.IO;

namespace Core
{
    public static class Translation
    {
        private static readonly SortedList<string, string> strings = new SortedList<string,string>();
        public static void Load()
        {
            string[] lines = File.ReadAllLines("config/translate/strings.ini");
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                int idx = line.IndexOf("=");
                if (idx >= 0)
                    strings.Add(line.Substring(0, idx), line.Substring(idx + 1));
            }
        }
        public static void Clear()
        {
            strings.Clear();
        }
        public static string GetLabel(string title)
        {
            try
            {
                if (strings.TryGetValue(title, out string value))
                    return value.Replace("\\n", ((char)0x0A).ToString());
                return title;
            }
            catch { return title; }
        }
        public static string GetLabel(string title, params object[] args) => string.Format(GetLabel(title), args);
    }
}