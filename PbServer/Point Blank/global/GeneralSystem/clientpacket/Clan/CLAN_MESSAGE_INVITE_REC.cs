using Core;
using Core.managers;
using Core.models.account;
using Core.models.enums;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_MESSAGE_INVITE_REC : ReceiveGamePacket
    {
        private uint erro;
        private int type;
        private long objId;
        public CLAN_MESSAGE_INVITE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            type = ReadC(); //0 - AMIGOS (Q) || 1 - SALA (D) || 2 - LOBBY (D)
            switch (type)
            {
                case 0:
                    {
                        objId = ReadQ(); break;
                    }
                case 1:
                    {
                        objId = ReadD(); break;
                    }
                case 2:
                    {
                        objId = ReadUD(); break;
                    }
                default: { ReadD(); break; }
            }
        }

        public override void Run()
        {
            Account p = _client._player;
            if (p == null || p.clanId == 0)
                return;
            try
            {
                switch (type)
                {
                    case 0:
                        {
                            Account f = AccountManager.GetAccount(objId, true);
                            if (f != null)
                                SendBoxMessage(f, p.clanId);
                            else erro = 0x80000000;
                            break;
                        }
                    case 1:
                        {
                            Room room = p._room;
                            if (room != null)
                            {
                                Account player = room.GetPlayerBySlot((int)objId);
                                if (player != null)
                                    SendBoxMessage(player, p.clanId);
                                else erro = 0x80000000;
                            }
                            else erro = 0x80000000;
                            break;
                        }
                    case 2:
                        {
                            Channel ch = p.GetChannel();
                            if (ch != null)
                            {
                                PlayerSession ps = ch.GetPlayer((uint)objId);
                                long pId = ps != null ? ps._playerId : -1;
                                if (pId != -1 && pId != _client.player_id)
                                {
                                    Account player = AccountManager.GetAccount(pId, true);
                                    if (player != null)
                                        SendBoxMessage(player, p.clanId);
                                    else erro = 0x80000000;
                                }
                                else erro = 0x80000000;
                            }
                            else erro = 0x80000000;
                            break;
                        }
                }
                _client.SendPacket(new CLAN_MESSAGE_INVITE_PAK(erro));
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
            }
        }
        private void SendBoxMessage(Account player, int clanId)
        {
            if (MessageManager.GetMsgsCount(player.player_id) >= 100)
                erro = 0x80000000;
            else
            {
                Message msg = CreateMessage(clanId, player.player_id, _client.player_id);
                if (msg != null && player._isOnline)
                    player.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(msg), false);
            }
        }
        private Message CreateMessage(int clanId, long owner, long senderId)
        {
            Message msg = new Message(15)
            {
                sender_name = ClanManager.GetClan(clanId)._name,
                clanId = clanId,
                sender_id = senderId,
                type = 5,
                state = 1,
                cB = NoteMessageClan.Invite
            };
            return MessageManager.CreateMessage(owner, msg) ? msg : null;
        }
    }
}