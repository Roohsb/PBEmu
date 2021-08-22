using Core;
using Core.managers;
using Core.models.enums;
using Core.server;
using Game.data.managers;
using Game.data.model;
using Game.global.serverpacket;
using NetFwTypeLib;
using System;
using System.Linq;

namespace Game.data.chat
{
    public static class Ban
    {
        public static INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        public static string UpdateReason(string str)
        {
            string text = str.Substring(7);
            int idx = text.IndexOf(" ");
            if (idx >= 0)
            {
                string[] split = text.Split(' ');
                long object_id = long.Parse(split[0]);
                string reason = text.Substring(idx + 1);
                if (!BanManager.SaveBanReason(object_id, reason))
                    return Translation.GetLabel("PlayerBanReasonFail");
                else
                    return Translation.GetLabel("PlayerBanReasonSuccess");

            }
            else
                return "Comando inválido. [Servidor]";
        }

        public static string BanForeverNick(string str, Account player)
        {
            string[] strs = str.Substring(6).Split(' ');
            string nick = strs[0];
            return BaseBanForever(player, AccountManager.GetAccount(nick, 1, 0));
        }
        
        public static string BanForeverId(string str, Account player)
        {
            string[] strs = str.Substring(7).Split(' ');
            long  playerid = long.Parse(strs[0]);
            return BaseBanForever(player, AccountManager.GetAccount(playerid, 0));
        }


        public static string BanNormalNick(string str, Account player)
        {
            string text = str.Substring(5);
            string[] split = text.Split(' ');
            string nick = split[0];
            double days = Convert.ToDouble(split[1]);
            DateTime endDate = DateTime.Now.AddDays(days);
            Account victim = AccountManager.GetAccount(nick, 1, 0);
            return BaseBanNormal(player, victim, endDate);
        }
        public static string BanNormalId(string str, Account player)
        {
            string text = str.Substring(6);
            string[] split = text.Split(' ');
            long player_id = Convert.ToInt64(split[0]);
            double days = Convert.ToDouble(split[1]);
            DateTime endDate = DateTime.Now.AddDays(days);
            Account victim = AccountManager.GetAccount(player_id, 0);
            return BaseBanNormal(player, victim,  endDate);
        }

        private static string BaseBanNormal(Account player, Account victim, DateTime endDate)
        {
            if (victim == null)
                return Translation.GetLabel("PlayerBanUserInvalid");
            else if (victim.access > player.access)
                return Translation.GetLabel("PlayerBanAccessInvalid");
            else if (player.player_id == victim.player_id)
                return Translation.GetLabel("PlayerBanSimilarID");
            else
            {
                BanHistory ban = BanManager.SaveHistory(player.player_id, "DURATION", victim.player_id.ToString(), endDate);
                if (ban != null)
                {
                    MsgToAll(victim.player_name);
                    if (victim._isOnline)
                    {
                        victim.ban_obj_id = ban.object_id;
                        victim.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                        victim.Close(1000, true);
                    }
                    return Translation.GetLabel("PlayerBanSuccess", ban.object_id);
                }
                else
                    return Translation.GetLabel("PlayerBanFail");
            }
        }
        private static string BaseBanForever(Account player, Account victim)
        {
            if (victim == null)
                return Translation.GetLabel("PlayerBanUserInvalid");
            else if (victim.access > player.access)
                return Translation.GetLabel("PlayerBanAccessInvalid");
            else if (player.player_id == victim.player_id)
                return Translation.GetLabel("PlayerBanSimilarID");
            else if (ComDiv.UpdateDB("accounts", "access_level", -1, "player_id", victim.player_id))
            {
                MsgToAll(victim.player_name);
                if (victim._isOnline)
                {
                    victim.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                    victim.Close(1000, true);
                }
                return "Player has been banned forever.";
            }
            else
                return Translation.GetLabel("PlayerBanFail");
        }

        public static string GetBanData(string str, Account player)
        {
            long id = long.Parse(str.Substring(7));
            BanHistory ban = BanManager.GetAccountBan(id);
            if (ban == null)
                return Translation.GetLabel("GetBanInfoError");
            string info = Translation.GetLabel("GetBanInfoTitle");
            info += "\n" + Translation.GetLabel("GetBanInfoProvider", ban.provider_id);
            info += "\n" + Translation.GetLabel("GetBanInfoType", ban.type);
            info += "\n" + Translation.GetLabel("GetBanInfoValue", ban.value);
            info += "\n" + Translation.GetLabel("GetBanInfoReason", ban.reason);
            info += "\n" + Translation.GetLabel("GetBanInfoStart", ban.startDate);
            info += "\n" + Translation.GetLabel("GetBanInfoEnd", ban.endDate);
            player.SendPacket(new SERVER_MESSAGE_ANNOUNCE_PAK(info));
            return Translation.GetLabel("GetBanInfoSuccess");
        }
        public static string BanConnection(string valor)
        {
            try
            {
                long player_id = long.Parse(valor);
                Account arc = AccountManager.GetAccount(player_id, 0);
                if (arc != null && !arc.IsGM())
                {
                    string IP = arc.PublicIP.ToString();
                    string strNick = "PlayerID Banido - " + arc.player_id;
                    bool bannedwins = ComDiv.UpdateDB("accounts", "access_level", -1, "player_id", arc.player_id);
                    if (bannedwins)
                    {
                        if (arc._isOnline)
                        {
                            arc.access = AccessLevel.antijogo;
                            arc.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
                            arc.Close(1000, true);
                        }
                        MsgToAll(arc.player_name);
                        INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == strNick).FirstOrDefault();
                        if (firewallRule == null)
                        {
                            firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                            firewallRule.Name = strNick;
                            firewallPolicy.Rules.Add(firewallRule);
                            firewallRule.Description = ""; // DESCRIÇÃO
                            firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL; // Todos os Perfil Publico ao privado
                            firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY; //Tipo de Protocolo
                            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // ENTRADA E SAIDA
                            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; //BLOQUEAR OU LIBERAR PACOTES
                            firewallRule.Enabled = true;//Ativar o rule
                            firewallRule.RemoteAddresses = IP;
                        }
                        else
                            firewallRule.RemoteAddresses = firewallRule.RemoteAddresses + "," + IP;
                        return "Eternally banned with any link to the server! " + IP + "";
                    }
                    else
                        return "Error when banning!";

                }
                else
                    return "This account does not exist.";
            }
            catch
            {
                return "Error processing ban.";
            }
        }
        //  public static string BanViaMac(string valor)
        //  {
        //     try
        //     {
        // Account arc = AccountManager.GetAccountDB(long.Parse(valor), 0);
        //       if (arc != null && !arc.IsGM())
        //        {
        //bool acpt = BanManager.banMacAdd(arc.MacAddress.ToString());
        // if (acpt)
        // {
        //     if (arc._isOnline)
        //          {
        //  arc.SendPacket(new AUTH_ACCOUNT_KICK_PAK(2), false);
        //     arc.Close(1000, true);
        //   }
        //   MsgToAll(arc.player_name);
        //                     return "Player Banned.";
        // }
        //                else
        //                    return "Player has not been banned.";
        // }
        //             else
        //   return "Player does not exist.";
        //       }
        //         catch
        //       {
        //             return "Error blocking mac. ";
        //        }
        //     }
        public static string RemoveBanConnection(string valor)
        {
            try
            {
                long player_id = long.Parse(valor);
                Account arc = AccountManager.GetAccount(player_id, 0);
                if (arc != null)
                {
                    bool bannedwins = ComDiv.UpdateDB("accounts", "access_level", 0, "player_id", arc.player_id);
                    if (bannedwins)
                    {
                        string strId = "PlayerID Banido - " + arc.player_id;
                        INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == (strId)).FirstOrDefault();
                        if (firewallRule != null)
                        {
                            SendDebug.SendInfo("Account It was unbanned and returned to have connection with the server. " + strId);
                            firewallPolicy.Rules.Remove(firewallRule.Name);
                            return "Account It was unbanned and returned to have connection with the server. but access -1 must be released " + arc.player_name;
                        }
                        else
                            return "There is no such nick in the system!";
                    }
                    else
                        return "Error unbanning the player.";
                }
                else
                    return "Banned account does not exist.";

            }
            catch
            {
                return "Error processing unbalance.";
            }
        }
        public static void MsgToAll(string nome)
        {
            string texto = $" {nome} Banned. ";
            using SERVER_MESSAGE_ANNOUNCE_PAK packet = new SERVER_MESSAGE_ANNOUNCE_PAK(texto);
            GameManager.SendPacketToAllClients(packet);
        }
    }
}