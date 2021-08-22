using Core;
using Core.models.enums.global;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CHAT_1390_REC : ReceiveGamePacket
    {
        private ChattingType type;
        private string text;
        public CLAN_CHAT_1390_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            type = (ChattingType)ReadH();
            text = ReadS(ReadH());
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || type != ChattingType.Clan_Member_Page)
                    return;
                using (CLAN_CHAT_1390_PAK packet = new CLAN_CHAT_1390_PAK(p, text))
                    ClanManager.SendPacket(packet, p.clanId, -1, true, true);
            }
            catch
            {
            }
        }
    }
}