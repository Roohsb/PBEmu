using Core;
using Core.managers;
using Core.models.account;
using Game.data.model;
using System;
using System.Collections.Generic;

namespace Game.global.Authentication
{
    public class BASE_USER_CONFIGS_REC : ReceiveGamePacket
    {
        public BASE_USER_CONFIGS_REC(GameClient client, byte[] data)
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
                if (p == null || p._myConfigsLoaded)
                    return;
                _client.SendPacket(new BASE_EXIT_URL_PAK(LoginManager.Config.ExitURL));
                if (p.FriendSystem._friends.Count > 0)
                    _client.SendPacket(new BASE_USER_FRIENDS_PAK(p.FriendSystem._friends));
                SendMessagesList(p);
                _client.SendPacket(new BASE_USER_CONFIG_PAK(0, p._config));
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BASE_USER_CONFIGS_REC] " + ex.ToString());
            }
        }
        private void SendMessagesList(Account p)
        {
            List<Message> msgs = MessageManager.GetMessages(p.player_id);
            if (msgs.Count == 0)
                return;
            MessageManager.RecicleMessages(p.player_id, msgs);
            if (msgs.Count == 0)
                return;
            for (int i = 0; i < (int)Math.Ceiling(msgs.Count / 25d); i++)
                _client.SendPacket(new BASE_USER_MESSAGES_PAK(i, msgs));
        }
    }
}