using Core;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class LOBBY_LEAVE_REC : ReceiveGamePacket
    {
        private uint erro;
        public LOBBY_LEAVE_REC(GameClient client, byte[] data)
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
                if (_client == null || _client._player == null)
                    return;
                Account player = _client._player;
                Channel channel = player.GetChannel();
                if (player._room != null || player._match != null)
                    return;
                if (channel == null || player.Session == null || !channel.RemovePlayer(player))
                    erro = 0x80000000;
                _client.SendPacket(new LOBBY_LEAVE_PAK(erro));
                if (erro == 0)
                {
                    player.ResetPages();
                    player._status.updateChannel(255);
                    AllUtils.SyncPlayerToFriends(player, false);
                    AllUtils.SyncPlayerToClanMembers(player);
                }
                else
                {
                    _client.Close(0, false);
                }
        
            }
            catch (Exception ex)
            {
                Logger.Info("LOBBY_LEAVE_REC: " + ex.ToString());
                _client.SendPacket(new LOBBY_LEAVE_PAK(0x80000000));
                _client.Close(0, false);
            }
        }
    }
}