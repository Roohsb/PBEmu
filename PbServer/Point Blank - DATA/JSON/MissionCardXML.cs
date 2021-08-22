using Core.models.account.mission;
using Core.models.account.players;
using Core.models.enums.item;
using Core.models.enums.missions;
using Core.server;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Core.xml
{
    public class MissionCardXML
    {
        private static List<MissionItemAward> _items = new List<MissionItemAward>();
        private static List<Card> list = new List<Card>();
        private static List<CardAwards> awards = new List<CardAwards>();
        private static void Load(string file, int type)
        {
            string path = "data/missions/deck/" + file + ".mqf";
            if (File.Exists(path))
                Parse(path, file, type);
            else
                Logger.Error("[MissionCardXML] Não existe o arquivo: " + path);
        }
        public static void LoadBasicCards(int type)
        {
            Load("AssaultCard", type);
            Load("Dino_Basic", type);
            Load("Dino_Intensify", type);
            Load("Human_Basic", type);
            Load("Human_Intensify", type);
            Load("TutorialCard", type);
            Load("Dino_Tutorial", type);
            Load("Human_Tutorial", type);
            Load("Field_o", type);
            Load("SpecialCard", type);
            Load("InfiltrationCard", type);
            Load("DefconCard", type);
            Load("Company_o", type);
            Load("BackUpCard", type);
            Load("Commissioned_o", type);
            Load("EventCard", type);
        }
        private static int ConvertStringToInt(string missionName)
        {
            int missionId = 0;
            switch (missionName)
            {
                case "TutorialCard": missionId = 1; break;
                case "Dino_Tutorial": missionId = 2; break;
                case "Human_Tutorial": missionId = 3; break;
                case "AssaultCard": missionId = 5; break;
                case "BackUpCard": missionId = 6; break;
                case "InfiltrationCard": missionId = 7; break;
                case "SpecialCard": missionId = 8; break;
                case "DefconCard": missionId = 9; break;
                case "Commissioned_o": missionId = 10; break;
                case "Company_o": missionId = 11; break;
                case "Field_o": missionId = 12; break;
                case "EventCard": missionId = 13; break;
                case "Dino_Basic": missionId = 14; break;
                case "Human_Basic": missionId = 15; break;
                case "Dino_Intensify": missionId = 16; break;
                case "Human_Intensify": missionId = 17; break;
            }
            return missionId;
        }
        public static List<ItemsModel> GetMissionAwards(int missionId)
        {
            List<ItemsModel> items = new List<ItemsModel>();
            lock (_items)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    MissionItemAward mia = _items[i];
                    if (mia._missionId == missionId)
                        items.Add(mia.item);
                }
            }
            return items;
        }
        public static List<Card> GetCards(int missionId, int cardBasicId)
        {
            List<Card> result = new List<Card>();
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Card card = list[i];
                    if (card._missionId == missionId && (cardBasicId >= 0 && card._cardBasicId == cardBasicId || cardBasicId == -1))
                        result.Add(card);
                }
            }
            return result;
        }
        public static List<Card> GetCards(List<Card> Cards, int cardBasicId)
        {
            if (cardBasicId == -1)
                return Cards;
            List<Card> result = new List<Card>();
            for (int i = 0; i < Cards.Count; i++)
            {
                Card card = Cards[i];
                if (cardBasicId >= 0 && card._cardBasicId == cardBasicId || cardBasicId == -1)
                    result.Add(card);
            }
            return result;
        }
        public static List<Card> GetCards(int missionId)
        {
            List<Card> result = new List<Card>();
            lock (list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Card card = list[i];
                    if (card._missionId == missionId)
                        result.Add(card);
                }
            }
            return result;
        }
        private static void Parse(string path, string missionName, int typeLoad)
        {
            int missionId = ConvertStringToInt(missionName);
            if (missionId == 0)
                Logger.Error("[INVALID]: " + missionName);
            byte[] buffer;
            try
            {
                buffer = File.ReadAllBytes(path);
            }
            catch
            {
                buffer = new byte[0];
            }
            if (buffer.Length == 0)
                return;
            try
            {
                ReceiveGPacket r = new ReceiveGPacket(buffer);
                r.ReadS(4);
                int questType = r.ReadD();
                r.ReadB(16);
                int valor1 = 0, valor2 = 0;
                for (int i = 0; i < 40; i++)
                {
                    int missionBId = valor2++,
                        cardBId = valor1;
                    if (valor2 == 4)
                    {
                        valor2 = 0;
                        valor1++;
                    }
                    int reqType = r.ReadUH();
                    int type = r.ReadC();
                    int mapId = r.ReadC();
                    byte limitCount = r.ReadC();
                    ClassType weaponClass = (ClassType)r.ReadC();
                    int weaponId = r.ReadUH();
                    Card nc = new Card(cardBId, missionBId)
                    {
                        _mapId = mapId,
                        _weaponReq = weaponClass,
                        _weaponReqId = weaponId,
                        _missionType = (MISSION_TYPE)type,
                        _missionLimit = limitCount,
                        _missionId = missionId
                    };
                    list.Add(nc);
                    if (questType == 1)
                        r.ReadB(24);
                }
                int vai = (questType == 2 ? 5 : 1);
                for (int i = 0; i < 10; i++)
                {
                    int gp = r.ReadD();
                    int xp = r.ReadD();
                    int medals = r.ReadD();
                    for (int i2 = 0; i2 < vai; i2++)
                    {
                        int unk = r.ReadD();
                        int type = r.ReadD();
                        int itemId = r.ReadD();
                        int itemCount = r.ReadD();
                    }
                    if (typeLoad == 1)
                    {
                        CardAwards card = new CardAwards { _id = missionId, _card = i, _exp = (questType == 1 ? (xp * 10) : xp), _gp = gp };
                        GetCardMedalInfo(card, medals);
                        if (!card.Unusable())
                            awards.Add(card);
                    }
                }
                if (questType == 2)
                {
                    int goldResult = r.ReadD();
                    r.ReadB(8);
                    for (int i = 0; i < 5; i++)
                    {
                        int unkI = r.ReadD();
                        int typeI = r.ReadD(); //1 - unidade | 2 - dias
                        int itemId = r.ReadD();
                        int itemCount = r.ReadD();
                        if (unkI > 0 && typeLoad == 1)
                            _items.Add(new MissionItemAward { _missionId = missionId, item = new ItemsModel(itemId) { _equip = 1, _count = (uint)itemCount, _name = "Mission item" } });
                    }
                }
            }
            catch (XmlException ex)
            {
                Logger.Error("[MissionCardXML] Erro no arquivo: " + path + "\r\n" + ex.ToString());
            }
        }
        private static void GetCardMedalInfo(CardAwards card, int medalId)
        {
            if (medalId == 0)
                return;
            if (medalId >= 1 && medalId <= 50) //v >= 1 && v <= 50
                card._brooch++;
            else if (medalId >= 51 && medalId <= 100) //v >= 51 && v <= 100
                card._insignia++;
            else if (medalId >= 101 && medalId <= 116) //v >= 101 && v <= 116
                card._medal++;
            //v >= 117 && v <= 239
        }
        public static CardAwards GetAward(int mission, int cartao)
        {
            for (int i = 0; i < awards.Count; i++)
            {
                CardAwards card = awards[i];
                if (card._id == mission && card._card == cartao)
                    return card;
            }
            return null;
        }
    }
    public class Card
    {
        public ClassType _weaponReq;
        public MISSION_TYPE _missionType;
        public int _missionId, _mapId, _weaponReqId, _missionLimit, _missionBasicId, _cardBasicId,
            _arrayIdx, _flag;
        public Card(int cardBasicId, int missionBasicId)
        {
            _cardBasicId = cardBasicId;
            _missionBasicId = missionBasicId;
            _arrayIdx = (_cardBasicId * 4) + _missionBasicId;
            _flag = (15 << 4 * _missionBasicId);
        }
    }
}