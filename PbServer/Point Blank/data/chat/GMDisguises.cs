using Core;
using Core.managers;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.data.utils;
using Game.global.serverpacket;
using System;

namespace Game.data.chat
{
    public static class GMDisguises
    {
        public static string SetHideColor(Account player)
        {
            if (player == null)
                return Translation.GetLabel("HideGMColorFail");
            if(player.HaveVipTotal())
            {
                return "You are a vip so you are not available.";
            }
            else if (player._rank == 53 || player._rank == 54)
            {
                player.HideGMcolor = !player.HideGMcolor;
                if (player.HideGMcolor)
                    return Translation.GetLabel("HideGMColorSuccess1");
                else
                    return Translation.GetLabel("HideGMColorSuccess2");
            }
            return Translation.GetLabel("HideGMColorNoRank");
        }
        public static string SetHideColorPlayer(Account c ,string strs)
        {
            try
            {
                string[] value = strs.Substring(8).Split(' ');
                long playerid = long.Parse(value[0]);
                int color = int.Parse(value[1]);
                if (playerid == 0)
                    return "Player cannot be 0.";
                else if (playerid == c.player_id || c.HaveAcessLevel())
                    return "You can't donate color";
                else if (color > 0 && color < 11)
                {
                    Account p = AccountManager.GetAccount(playerid, 0);
                    if (p != null && p._isOnline)
                    {
                        if (ComDiv.UpdateDB("accounts", "name_color", color, "player_id", p.player_id))
                        {
                            p.SendPacket(new AUTH_ACCOUNT_KICK_PAK(0));
                            p.Close(1000);
                            return "Color set for the player successfully.";
                        }
                        else
                        {
                            return "Error adding the color for the player.";
                        }

                    }
                    else
                        return "Player does not exist, or is offline.";
                }
                else
                    return "Invalid Color!";
            }
            catch
            {
                return "Error found.";
            }
        }

        public static string SetAntiKick(Account player)
        {
            if (player == null)
                return Translation.GetLabel("AntiKickGMFail");
            player.AntiKickGM = !player.AntiKickGM;
            if (player.AntiKickGM)
                return Translation.GetLabel("AntiKickGMSuccess1");
            else
                return Translation.GetLabel("AntiKickGMSuccess2");
        }

        public static string SetFakeRank(string str, Account player, Room room)
        {
            try
            {
                int rank = int.Parse(str.Substring(11));
                if (rank > 55 || rank < 0)
                    return Translation.GetLabel("FakeRankWrongValue");
                else if (player._bonus.fakeRank == rank)
                    return Translation.GetLabel("FakeRankAlreadyFaked");
                else if (ComDiv.UpdateDB("player_bonus", "fakerank", rank, "player_id", player.player_id))
                {
                    player._bonus.fakeRank = rank;
                    player.SendPacket(new BASE_USER_EFFECTS_PAK(0, player._bonus));
                    if (room != null)
                        room.UpdateSlotsInfo();
                    if (rank == 55)
                        return Translation.GetLabel("FakeRankSuccess1");
                    else
                        return Translation.GetLabel("FakeRankSuccess2", rank);
                }
                return Translation.GetLabel("FakeRankFail");
            }
            catch(Exception)
            {
                return "Error creating fake patent.";
            }
        }
        public static string SetFakeNick(string str, Account player, Room room)
        {
            try
            {
                string name = str.Substring(11);
                if (name.Length > Settings.maxNickSize || name.Length < Settings.minNickSize)
                    return Translation.GetLabel("FakeNickWrongLength");
                else if (PlayerManager.IsPlayerNameExist(name))
                    return Translation.GetLabel("FakeNickAlreadyExist");
                else if (ComDiv.UpdateDB("accounts", "player_name", name, "player_id", player.player_id))
                {
                    player.player_name = name;
                    player.SendPacket(new AUTH_CHANGE_NICKNAME_PAK(name));
                    if (room != null)
                        using (ROOM_GET_NICKNAME_PAK packet = new ROOM_GET_NICKNAME_PAK(player._slotId, player.player_name, player.name_color))
                            room.SendPacketToPlayers(packet);
                    if (player.clanId > 0)
                    {
                        using CLAN_MEMBER_INFO_UPDATE_PAK packet = new CLAN_MEMBER_INFO_UPDATE_PAK(player);
                        ClanManager.SendPacket(packet, player.clanId, -1, true, true);
                    }
                    AllUtils.SyncPlayerToFriends(player, true);
                    return Translation.GetLabel("FakeNickSuccess", name);
                }
                return Translation.GetLabel("FakeNickFail");
            }
            catch (Exception)
            {
                return "Error creating nick fake.";
            }
        }
    }
}