using Core;
using Core.managers;
using Core.models.account.clan;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_REQUEST_LIST_REC : ReceiveGamePacket
    {
        private int page;
        public CLAN_REQUEST_LIST_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            page = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account player = _client._player;
                if (player == null)
                    return;
                if (player.clanId == 0)
                {
                    _client.SendPacket(new CLAN_REQUEST_LIST_PAK(-1));
                    return;
                }
                List<ClanInvite> clanInvites = PlayerManager.GetClanRequestList(player.clanId);
                using (SendGPacket p = new SendGPacket())
                {
                    int count = 0;
                    for (int i = (page * 13); i < clanInvites.Count; i++)
                    {
                        WriteData(clanInvites[i], p);
                        if (++count == 13)
                            break;
                    }
                    _client.SendPacket(new CLAN_REQUEST_LIST_PAK(0, count, page, p.mstream.ToArray()));
                }
            }
            catch (Exception ex)
            {
                Logger.Info("CLAN_REQUEST_LIST_REC: " + ex.ToString());
            }
        }
        private void WriteData(ClanInvite invite, SendGPacket p)
        {
            p.writeQ(invite.player_id);
            Account pl = AccountManager.GetAccount(invite.player_id, 0);
            if (pl != null)
            {
                p.writeS(pl.player_name, 33);
                p.writeC((byte)pl._rank);
            }
            else
                p.writeB(new byte[34]);
            p.writeD(invite.inviteDate);
        }
    }
}