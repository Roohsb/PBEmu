using Core;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class CLAN_WAR_LEAVE_TEAM_REC : ReceiveGamePacket
    {
        private uint erro;
        public CLAN_WAR_LEAVE_TEAM_REC(GameClient client, byte[] data)
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
                Match mt = p == null ? null : p._match;
                if (mt == null || !mt.RemovePlayer(p))
                    erro = 0x80000000;
                _client.SendPacket(new CLAN_WAR_LEAVE_TEAM_PAK(erro));
                if (erro == 0)
                {
                    p._status.updateClanMatch(255);
                    AllUtils.SyncPlayerToClanMembers(p);
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}