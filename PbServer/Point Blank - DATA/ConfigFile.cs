using System;
using System.Collections.Generic;
using System.IO;

namespace Core
{
    public class ConfigFile
    {
        private FileInfo File;
        private SortedList<string, string> _topics;

        public ConfigFile(string path)
        {
            try
            {
                File = new FileInfo(path);
                _topics = new SortedList<string, string>();
                LoadStrings();
            }
            catch (Exception ex)
            {
                Logger.Error("[ConfigFile] " + ex.ToString());
            }
        }

        private void LoadStrings()
        {
            try
            {
                using (StreamReader reader = new StreamReader(File.FullName))
                {
                    while (!reader.EndOfStream)
                    {
                        string str = reader.ReadLine();
                        if (str.Length != 0 && !str.StartsWith(";") && !str.StartsWith("["))
                        {
                            string[] split = str.Split('=');
                            _topics.Add(split[0], split[1]);
                        }
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
        public float ReadFloat(string value, float defaultprop)
        {
            float str;
            try
            {
                str = float.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public bool ReadBoolean(string value, bool defaultprop)
        {
            bool str;
            try
            {
                str = bool.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public long ReadInt64(string value, long defaultprop)
        {
            long str;
            try
            {
                str = long.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public ulong ReadUInt64(string value, ulong defaultprop)
        {
            ulong str;
            try
            {
                str = ulong.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public int ReadInt32(string value, int defaultprop)
        {
            int str;
            try
            {
                str = int.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public uint ReadUInt32(string value, uint defaultprop)
        {
            uint str;
            try
            {
                str = uint.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public ushort ReadUInt16(string value, ushort defaultprop)
        {
            ushort str;
            try
            {
                str = ushort.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public byte ReadByte(string value, byte defaultprop)
        {
            byte str;
            try
            {
                str = byte.Parse(_topics[value]);
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str;
        }
        public string ReadString(string value, string defaultprop)
        {
            string str;
            try
            {
                str = _topics[value];
            }
            catch
            {
                Error(value);
                return defaultprop;
            }
            return str ?? defaultprop;
        }
        private void Error(string parameter)
        {
            Logger.Error("[ConfigFile] Falha no parâmetro: " + parameter);
        }
    }
}