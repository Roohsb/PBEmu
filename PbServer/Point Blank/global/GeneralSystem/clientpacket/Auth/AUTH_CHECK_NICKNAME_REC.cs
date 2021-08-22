using Core;
using Core.managers;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class AUTH_CHECK_NICKNAME_REC : ReceiveGamePacket
    {
        private string name;
        public AUTH_CHECK_NICKNAME_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            name = ReadS(33);
        }

        public override void Run()
        {
            try
            {
                if (_client == null || _client._player == null)
                    return;
                _client.SendPacket(new AUTH_CHECK_NICKNAME_PAK(!PlayerManager.IsPlayerNameExist(name) ? 0 : 2147483923));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}