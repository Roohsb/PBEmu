using Core;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class AUTH_FIND_USER_REC : ReceiveGamePacket
    {
        private string name;
        public AUTH_FIND_USER_REC(GameClient client, byte[] data)
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
                Account p = _client._player;
                if (p == null || p.player_name.Length == 0 || p.player_name == name)
                    return;
                Account user = AccountManager.GetAccount(name, 1, 0);
                _client.SendPacket(new AUTH_FIND_USER_PAK(user == null ? 2147489795 : !user._isOnline ? 2147489796 : 0, user));
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }
    }
}