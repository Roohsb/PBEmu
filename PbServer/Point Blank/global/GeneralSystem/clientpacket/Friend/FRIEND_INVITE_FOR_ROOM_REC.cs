using Core;
using Core.models.account;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class FRIEND_INVITE_FOR_ROOM_REC : ReceiveGamePacket
    {
        private int index;
        public FRIEND_INVITE_FOR_ROOM_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            index = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                Account fr = GetFriend(p);
                if (fr != null)
                {
                    if (fr._status.serverId == 255 || fr._status.serverId == 0)
                    {
                        _client.SendPacket(new FRIEND_INVITE_FOR_ROOM_PAK(0x80003002));
                        return;
                    }
                    else if (fr.matchSlot >= 0)
                    {
                        _client.SendPacket(new FRIEND_INVITE_FOR_ROOM_PAK(0x80003003));
                        return;
                    }
                    int pIdx = fr.FriendSystem.GetFriendIdx(p.player_id);
                    if (pIdx == -1)
                        _client.SendPacket(new FRIEND_INVITE_FOR_ROOM_PAK(0x8000103E));
                    else if (fr._isOnline)
                        fr.SendPacket(new FRIEND_ROOM_INVITE_PAK(pIdx), false);
                    else
                        _client.SendPacket(new FRIEND_INVITE_FOR_ROOM_PAK(0x8000103F));
                }
                else
                    _client.SendPacket(new FRIEND_INVITE_FOR_ROOM_PAK(0x8000103D));
            }
            catch (Exception ex)
            {
                Logger.Info("[FRIEND_INVITE_FOR_ROOM_REC] " + ex.ToString());
            }
        }
        private Account GetFriend(Account p)
        {
            Friend friend = p.FriendSystem.GetFriend(index);
            if (friend == null)
                return null;
            return AccountManager.GetAccount(friend.player_id, 32);
        }
    }
}