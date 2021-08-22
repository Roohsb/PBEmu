using Core;
using Core.managers;
using Core.models.account;
using Core.models.account.clan;
using Core.models.enums;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_PROMOTE_AUX_REC : ReceiveGamePacket
    {
        private uint result;
        public CLAN_PROMOTE_AUX_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            Account player = _client._player;
            if (player == null)
                return;
            Clan clan = ClanManager.GetClan(player.clanId);
            if (clan._id == 0 || !(player.clanAccess == 1 || clan.owner_id == _client.player_id))
            {
                result = 2147487833;
                return;
            }
            int countPlayers = ReadC();
            for (int i = 0; i < countPlayers; i++)
            {
                Account member = AccountManager.GetAccount(ReadQ(), 0);
                if (member != null && member.clanId == clan._id && member.clanAccess == 3 && ComDiv.UpdateDB("accounts", "clanaccess", 2, "player_id", member.player_id))
                {
                    member.clanAccess = 2; //Aux
                    SEND_CLAN_INFOS.Load(member, null, 3);
                    if (MessageManager.GetMsgsCount(member.player_id) < 100)
                    {
                        Message msg = CreateMessage(clan, member.player_id, _client.player_id);
                        if (msg != null && member._isOnline)
                            member.SendPacket(new BOX_MESSAGE_RECEIVE_PAK(msg), false);
                    }
                    if (member._isOnline)
                        member.SendPacket(new CLAN_PRIVILEGES_AUX_PAK(), false);
                    result++;
                }
            }
        }

        public override void Run()
        {
            try
            {
                if (_client != null)
                    _client.SendPacket(new CLAN_COMMISSION_STAFF_PAK(result));
            }
            catch (Exception ex)
            {
                Logger.Info("[CLAN_PROMOTE_AUX_REC] " + ex.ToString());
            }
        }
        private Message CreateMessage(Clan clan, long owner, long senderId)
        {
            Message msg = new Message(15)
            {
                sender_name = clan._name,
                sender_id = senderId,
                clanId = clan._id,
                type = 4,
                state = 1,
                cB = NoteMessageClan.Staff
            };
            return MessageManager.CreateMessage(owner, msg) ? msg : null;
        }
    }
}