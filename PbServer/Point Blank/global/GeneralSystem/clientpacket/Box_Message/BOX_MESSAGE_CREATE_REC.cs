using Core;
using Core.managers;
using Core.models.account;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BOX_MESSAGE_CREATE_REC : ReceiveGamePacket
    {
        private int nameLength, textLength;
        private string name, text;
        private uint erro;
        public BOX_MESSAGE_CREATE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            nameLength = ReadC();
            textLength = ReadC();
            name = ReadS(nameLength);
            text = ReadS(textLength);
        }

        public override void Run()
        {
            try
            {
                if (text.Length > 120)
                    return;
                //0x80001080 STR_TBL_NETWORK_DONT_SEND_MYSELF_MESSAGE
                //0x80001081 STR_TBL_NETWORK_FULL_SEND_MESSAGE_PER_DAY
                Account p = _client._player;
                if (p == null || p.player_name.Length == 0 || p.player_name == name)
                    return;
                Account rec = AccountManager.GetAccount(name, 1, 0);
                if (rec != null)
                {
                    if (MessageManager.GetMsgsCount(rec.player_id) >= 100)
                        erro = 0x8000107F;
                    else
                    {
                        Message msg = CreateMessage(p.player_name, rec.player_id, _client.player_id);
                        if (msg != null)
                            rec.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(msg), false);
                    }
                }
                else erro = 0x8000107E;
                _client.SendPacket(new BOX_MESSAGE_CREATE_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info("[BOX_MESSAGE_CREATE_REC] " + ex.ToString());
            }
        }
        private Message CreateMessage(string senderName, long owner, long senderId)
        {
            Message msg = new Message(15)
            {
                sender_name = senderName,
                sender_id = senderId,
                text = text,
                state = 1
            };
            if (!MessageManager.CreateMessage(owner, msg))
            {
                erro = 0x80000000;
                return null;
            }
            return msg;
        }
    }
}