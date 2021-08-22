using Core.DB_Battle;
using Core.managers;
using Core.managers.server;
using Core.models.enums;
using Core.models.enums.errors;
using Core.models.enums.global;
using Core.models.enums.Login;
using Core.models.servers;
using Core.server;
using Core.xml;
using Game.data.managers;
using Game.data.model;
using Game.data.sync;
using Game.data.sync.server_side;
using Game.global.serverpacket;
using Game.Progress;
using IpPublicKnowledge;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.NetworkInformation;

namespace Game.global.Authentication
{
    public class BASE_LOGIN_REC : ReceiveGamePacket
    {
        private string login, passN, passEnc, GameVersion, PublicIP, LocalIP;
        private IsRealiP IsRealIP;
        private ulong key;
        private ClientLocale GameLocale;
        // private PhysicalAddress MacAddress;
        private DateTime DatadeCriaçao;
        public IPI acs;

        public BASE_LOGIN_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }
        public override void Read()
        {
            GameVersion = ReadC() + "." + ReadH() + "." + ReadH();
            GameLocale = (ClientLocale)ReadC();
            byte loginsize = ReadC();
            byte passwordsize = ReadC();
            login = ReadS(loginsize);
            passN = ReadS(passwordsize);
            ReadB(6);
            ReadH();
            IsRealIP = (IsRealiP)ReadC();
            LocalIP = new IPAddress(ReadB(4)).ToString();
            key = ReadUQ();
            ReadS(32);
            ReadD();
            ReadS(32);
            PublicIP = _client.GetIPAddress();
            DatadeCriaçao = DateTime.Now;
            passEnc = ComDiv.Gen5(passN);
        }
        public override void Run()
        {
            try
            {
                if (LoginQueue())
                    return;
                if (Settings.LauncherKey > 0 && key != Settings.LauncherKey)
                {
                    SendDebug.SendInfo("key: " + key + " não é compativel [" + login + "]");
                    return;
                }
                if (!LoginManager.RemoveBanned(_client))
                    return;
                if (LoginManager.Config == null)
                {
                    SendDebug.SendInfo("Invalid server configuration [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_User_Break, login, 0));
                    return;
                }
                else if (Settings.LocalDoGame != GameLocale)
                {
                    SendDebug.SendInfo("Customer country blocked: " + GameLocale + " [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_COUNTRY, login, 0));
                    return;
                }
                else if (login.Length < 5)
                {
                    SendDebug.SendInfo("Login too small [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ID_PASS_INCORRECT, login, 0));
                    return;
                }
                else if (passN.Length < 5)
                {
                    SendDebug.SendInfo("password too small [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ID_PASS_INCORRECT, login, 0));
                    return;
                }
                else if (login.Length > 16)
                {
                    SendDebug.SendInfo("Login too big [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_User_Break, login, 0));
                    return;
                }
                else if (passN.Length > 16)
                {
                    SendDebug.SendInfo("password too big [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_User_Break, login, 0));
                    return;
                }
                // else if (MacAddress.GetAddressBytes() == new byte[6])
                //  {
                //   SendDebug.SendInfo("MAC invalid. [" + login + "]");
                //   _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_User_Break, login, 0));
                //   return;
                // }
                //     else if (MacAddress.ToString() == "000000000000")
                //     {
                //         SendDebug.SendInfo("MAC invalid:  000000000000 [" + login + "]");
                //      _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_DB_BUFFER_FAIL, login, 0));
                //      return;
                // }
                else if (GameVersion != LoginManager.Config.ClientVersion)
                {
                    SendDebug.SendInfo("Unsupported version: " + GameVersion + " [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_Failed_Default, login, 0));
                    return;
                }
                else if (IsRealIP == IsRealiP.Host && (!IPAddress.TryParse(LocalIP, out IPAddress LocalAddr) || !IPAddress.TryParse(PublicIP, out IPAddress RemoteAddr)))
                {
                    SendDebug.SendInfo("Invalid IP: " + LocalIP + " ~ " + PublicIP + " [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_User_Break, login, 0));
                    return;
                }
                else if (IsRealIP == IsRealiP.Host && (LocalIP == null || LocalIP == "127.0.0.1" || LocalIP == "0" || LocalIP == "0.0.0.0" || LocalIP.StartsWith("0")))
                {
                    SendDebug.SendInfo("Null IP: " + LocalIP + "  [" + login + "]");
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_IP, login, 0));
                    return;
                }
                if (!AceptedCountry()) return;
                _client._player = AccountManager.GetAccountDB(login, 0, 0);
                if (_client._player == null && Settings.AUTO_ACCOUNTS && !AccountManager.CreateAccount(out _client._player, login, passEnc, PublicIP, DatadeCriaçao))
                {
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_User_Break, login, 0));
                    SendDebug.SendInfo("Failed to create account automatically [" + login + "]");
                }
                else
                {
                    Account p = _client._player;
                    if (p == null || !p.ComparePassword(passEnc))
                    {
                        string msg = p == null ? "Returned DB account is null" : "invalid password";
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_ID_PASS_INCORRECT, login, 0));
                        SendDebug.SendInfo(msg + " [" + login + "]");
                    }
                    else if (p.access < 0)
                    {
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_ACCOUNT, login, 0));
                        SendDebug.SendInfo("Permanent banned [" + login + "]");
                    }
                    //   else if (BanManager.selectbanmac(p.MacAddress.ToString()))
                    //  {
                    //  _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_PC_BLOCK, login, 0));
                    //      SendDebug.SendInfo("Permanently banned by mac [" + login + "]");
                    // }
                    else if (p.access >= 0 && (int)p.access <= 6)
                    {
                        if (p.IsGM() && LoginManager.Config.onlyGM || p.access >= 0 && !LoginManager.Config.onlyGM)
                        {

                            BanHistory htb = BanManager.GetAccountBan(p.ban_obj_id);
                            if (htb.endDate > DateTime.Now)
                            {
                                _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_ACCOUNT, login, 0));
                                SendDebug.SendInfo("Account with active ban [" + login + "]");
                            }
                            else
                            {
                                Account account = AccountManager.GetAccount(p.player_id, true);
                                if (account._isOnline && account._connection != null)
                                {
                                    using (BASE_LOGIN_PAK pLogin = new BASE_LOGIN_PAK(EventErrorEnum.Login_ALREADY_LOGIN_WEB, login, 0))
                                        _client.SendPacket(pLogin);
                                    SendDebug.SendInfo("Account was already online and connection was made in the same [" + login + "].");
                                    account.SendPacket(new AUTH_ACCOUNT_KICK_PAK(1));
                                    account.Close(1000);
                                }
                                else
                                {
                                    bool clean = CleanComp.AccountClean(p, LocalIP, PublicIP);
                                    if (clean)
                                        return;
                                    p.SetPlayerId(p.player_id, 31);
                                    p._clanPlayers = ClanManager.GetClanPlayers(p.clanId, p.player_id, false);
                                    using (BASE_LOGIN_PAK packet = new BASE_LOGIN_PAK(0, login, p.player_id))
                                        _client.SendPacket(packet);
                                    using (AUTH_WEB_CASH_PAK WebPacket = new AUTH_WEB_CASH_PAK(0))
                                        _client.SendPacket(WebPacket);
                                    if (p.clanId > 0)
                                        _client.SendPacket(new BASE_USER_CLAN_MEMBERS_PAK(p._clanPlayers));
                                    p._status.SetData(4294967295, p.player_id);
                                    p._status.updateServer(0);
                                    p.SetOnlineStatus(true);
                                    string addr = _client.GetIPAddress();
                                    SEND_REFRESH_ACC.RefreshAccount(p, true);
                                    if (p.player_name == "")
                                        SendDebug.SendConta(login, p.player_id, p.GetSessionId(), PublicIP, p.access.ToString(), IsRealIP.ToString(), NextModel.Pais(_client.GetAddress()));
                                }
                            }
                        }
                        else
                        {
                            _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_TIME_OUT_2, login, 0));
                            SendDebug.SendInfo("Invalid access level [" + login + "]");
                        }
                    }
                    else
                    {
                        _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_TIME_OUT_2, login, 0));
                        SendDebug.SendInfo("Access Level greater than 6 [" + login + "]");
                    }
                }
            }
            catch (Exception ex)
            {
                SendDebug.SendInfo("[BASE_LOGIN_REC] " + ex.ToString());
            }
        }

        private bool LoginQueue()
        {
            GameServerModel server = ServersXML.getServer(0);
            if (server._LastCount >= server._maxPlayers)
            {
                if (LoginManager._loginQueue.Count >= 100)
                {
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_SERVER_USER_FULL, login, 0));
                    SendDebug.SendInfo("Full server [" + login + "]");
                    return true;
                }
                else
                {
                    int pos = LoginManager.EnterQueue(_client);
                    _client.SendPacket(new A_LOGIN_QUEUE_PAK(pos + 1, ((pos + 1) * 120)));
                }
            }
            return false;
        }
        public bool AceptedCountry()
        {
            try
            {
                acs = IPK.GetIpInfo(_client.GetAddress());
                if (Settings.IP_Jogo != "127.0.0.1" && acs.country != "Brazil" && acs.country != "Thailand" && acs.country != "Turkey" && acs.country != "Germany" && acs.country != "Portugal" && acs.country != "Mexico" && acs.country != "Spain" && acs.country != "Italy")
                {
                    Console.WriteLine("Locked Continent: " + acs.country + " Login:" + login);
                    _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_COUNTRY, login, 0));
                    return false;
                }
                return true;
            }
            catch
            {
                _client.SendPacket(new BASE_LOGIN_PAK(EventErrorEnum.Login_BLOCK_COUNTRY, login, 0));
                return false;
            }
        }
    }
}