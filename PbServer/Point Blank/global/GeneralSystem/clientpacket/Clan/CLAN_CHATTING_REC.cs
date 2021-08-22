using Core.models.enums.global;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System.Diagnostics;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CHATTING_REC : ReceiveGamePacket
    {
        private ChattingType type;
        private string text;
        public CLAN_CHATTING_REC(GameClient client, byte[] data)
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
                if (p == null || text.Length > 60 || type != ChattingType.Clan)
                    return;
                using CLAN_CHATTING_PAK packet = new CLAN_CHATTING_PAK(text, p);
                ClanManager.SendPacket(packet, p.clanId, -1, true, true);
            }
            catch
            {
            }
        }
    }
}