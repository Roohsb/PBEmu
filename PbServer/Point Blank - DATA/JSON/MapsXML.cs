using Core.models.room;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.xml
{
    public class MapsXML
    {
        public static List<byte> TagList = new List<byte>();
        public static List<ushort> ModeList = new List<ushort>();
        public static ConcurrentDictionary<int, Map> maps = new ConcurrentDictionary<int, Map>();
        public static uint maps1, maps2, maps3, maps4;
        public static void Load()
        {
            string path = "data/maps/modes.xml";
            if (File.Exists(path))
                Parse(path);
            else
                Logger.Error("[MapsXML] There is no file: " + path);
        }
        private static void Parse(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                if (fileStream.Length == 0)
                    Logger.Error("[MapsXML] There is no file: " + path);
                else
                {//DESBUGAR MODO ZOMBIE QUE SÓ CARREGA HIDRANCE
                    //MUDAR VALORES DOS DINOS
                    try
                    {
                        int idx = 0, list2 = 1, mapaid = 0;
                        xmlDocument.Load(fileStream);
                        for (XmlNode xmlNode1 = xmlDocument.FirstChild; xmlNode1 != null; xmlNode1 = xmlNode1.NextSibling)
                        {
                            if ("list".Equals(xmlNode1.Name))
                            {
                                for (XmlNode xmlNode2 = xmlNode1.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
                                {
                                    if ("map".Equals(xmlNode2.Name))
                                    {
                                        XmlNamedNodeMap xml = xmlNode2.Attributes;
                                        uint flag = (uint)(1 << idx++);
                                        int list = list2;
                                        if (idx == 32) { list2++; idx = 0; }
                                        TagList.Add(byte.Parse(xml.GetNamedItem("tag").Value));
                                        ModeList.Add(ushort.Parse(xml.GetNamedItem("mode").Value));
                                        bool enable = bool.Parse(xml.GetNamedItem("enable").Value);
                                        if (!enable)
                                            continue;

                                        ++mapaid;

                                        maps.TryAdd(mapaid, new Map
                                        {
                                            _id = mapaid,
                                            act = enable
                                        });

                                        switch (list)
                                        {
                                            case 1: maps1 += flag; break;
                                            case 2: maps2 += flag; break;
                                            case 3: maps3 += flag; break;
                                            case 4: maps4 += flag; break;
                                            default:
                                                Logger.Error("[List not defined] Flag: " + flag + "; List: " + list);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (XmlException ex)
                    {
                        Logger.Error(ex.ToString());
                    }
                }
                fileStream.Dispose();
                fileStream.Close();
            }
        }
    }
}