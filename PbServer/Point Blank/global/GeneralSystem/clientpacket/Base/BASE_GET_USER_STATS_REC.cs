using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_GET_USER_STATS_REC : ReceiveGamePacket
    {
        private long objId;
        public BASE_GET_USER_STATS_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            objId = ReadQ();
        }

        public override void Run()
        {
            if (_client._player == null)
                return;
            try
            {
                Account player = AccountManager.GetAccount(objId, 0);
                _client.SendPacket(new BASE_GET_USER_STATS_PAK(player?._statistic));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}