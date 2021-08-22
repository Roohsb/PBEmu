using Core;
using Core.managers;
using Core.models.account.clan;
using Core.models.enums;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_CLIENT_ENTER_REC : ReceiveGamePacket
    {
        private int id;
        public CLAN_CLIENT_ENTER_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Room room = p._room;
                if (room != null)
                {
                    room.ChangeSlotState(p._slotId, SLOT_STATE.CLAN, false);
                    room.StopCountDown(p._slotId);
                    room.UpdateSlotsInfo();
                }
                Clan clan = ClanManager.GetClan(p.clanId);
                if (p.clanId == 0 && p.player_name.Length > 0)
                    id = PlayerManager.GetRequestClanId(p.player_id);
                _client.SendPacket(new CLAN_CLIENT_ENTER_PAK(id > 0 ? id : clan._id, p.clanAccess));
                if (clan._id > 0 && id == 0)
                    _client.SendPacket(new CLAN_DETAIL_INFO_PAK(0, clan));
            }
            catch (Exception ex)
            {
                Logger.Info("CLAN_CLIENT_ENTER_REC: " + ex.ToString());
            }
        }
    }
}