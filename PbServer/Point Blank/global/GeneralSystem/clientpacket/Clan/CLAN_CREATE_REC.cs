using Core;
using Core.managers;
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
    public class CLAN_CREATE_REC : ReceiveGamePacket
    {
        private uint erro;
        private string clanName, clanInfo, clanAzit;
        public CLAN_CREATE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            byte NameLength = ReadC();
            byte InfoLength = ReadC();
            byte AzitLength = ReadC();
            clanName = ReadS(NameLength);
            clanInfo = ReadS(InfoLength);
            clanAzit = ReadS(AzitLength);
            ReadD();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Clan clan = new Clan
                {
                    _name = clanName,
                    _info = clanInfo,
                    _logo = 0,
                    owner_id = p.player_id,
                    creationDate = int.Parse(DateTime.Now.ToString("yyyyMMdd"))
                };
                if (p.clanId > 0 || PlayerManager.GetRequestClanId(p.player_id) > 0)
                    erro = 0x8000105C;
                else if (0 > p._gp - Settings.minCreateGold && p.access < AccessLevel.Moderator || Settings.minCreateRank > p._rank && p.access < AccessLevel.Moderator)
                    erro = 0x8000104A;
                else if (ClanManager.IsClanNameExist(clan._name))
                {
                    erro = 0x8000105A;
                    return;
                }
                else if (ClanManager._clans.Count > Settings.maxActiveClans)
                    erro = 0x80001055;
                else if (PlayerManager.CreateClan(out clan._id, clan._name, clan.owner_id, clan._info, clan.creationDate) && PlayerManager.UpdateAccountGold(p.player_id, p._gp - Settings.minCreateGold))
                {
                    clan.BestPlayers.SetDefault();
                    p.clanDate = clan.creationDate;
                    if (ComDiv.UpdateDB("accounts", "player_id", p.player_id, new string[] { "clanaccess", "clandate", "clan_id" }, 1, clan.creationDate, clan._id))
                    {
                        if (clan._id > 0)
                        {
                            p.clanId = clan._id;
                            p.clanAccess = 1;
                            ClanManager.AddClan(clan);
                            SEND_CLAN_INFOS.Load(clan, 0);
                            p._gp -= Settings.minCreateGold;
                        }
                        else erro = 0x8000104B;
                    }
                    else erro = 0x80001048;
                }
                else erro = 0x80001048;
                _client.SendPacket(new CLAN_CREATE_PAK(erro, clan, p));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[CLAN_CREATE_REC] " + ex.ToString());
            }
        }
    }
}