using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class AUTH_SEND_WHISPER2_REC : ReceiveGamePacket
    {
        private long receiverId;
        private string receiverName, text;
        public AUTH_SEND_WHISPER2_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            receiverId = ReadQ();
            receiverName = ReadS(33);
            text = ReadS(ReadH());
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null || p.player_id == receiverId || p.player_name == receiverName)
                    return;
                Account pW = AccountManager.GetAccount(receiverId, 0);
                if (pW == null || pW.player_name != receiverName || !pW._isOnline)
                    _client.SendPacket(new AUTH_SEND_WHISPER_PAK(receiverName, text, 0x80000000));
                else
                    pW.SendPacket(new AUTH_RECV_WHISPER_PAK(p.player_name, text, p.UseChatGM()), false);
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}