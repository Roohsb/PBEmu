using Core;
using Core.models.enums.gameserver;
using Core.server;
using Game.data.model;
using Game.data.sync;
using Game.data.sync.server_side;
using Game.data.utils;
using Game.global;
using Game.global.Authentication;
using Game.global.clientpacket;
using Game.global.GeneralSystem.clientpacket;
using Game.global.serverpacket;
using Game.Progress;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using IntelliLock.Licensing;
using Game.global.GeneralSystem.clientpacket.Lorentudio;

namespace Game
{
    public class GameClient : IDisposable
    {
        public long player_id;
        public Socket _client;
        public Account _player;
        public DateTime ConnectDate;
        public uint SessionId;
        public int Shift, CryptSeed;
        public FistPacket_ID firstPacketId;
        bool disposed = false, BreakCache = false;
        readonly SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public ushort SessionSeed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            _player = null;
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
            player_id = 0;
            if (disposing)
                handle.Dispose();
            disposed = true;
        }
        public GameClient(Socket client)
        {
            _client = client;
        }
        public void Start()
        {
            SessionSeed = (ushort)new Random().Next(0, short.MaxValue);
            CryptSeed = SessionSeed;
            Shift = (int)(SessionId % 7 + 1);
            new Thread(InitiGame).Start();
            new Thread(Read).Start();
            new Thread(ConnectionCheck).Start();
            ConnectDate = DateTime.Now;
        }
        /*** Get the license. ***/
        public byte[] GetLicense()
        {
            return EvaluationMonitor.GetCurrentLicenseAsByteArray();
        }


        /*** Read additonal license information from a license file ***/
        public void ReadAdditonalLicenseInformation()
        {
            /* Check first if a valid license file is found */
            if (EvaluationMonitor.CurrentLicense.LicenseStatus == IntelliLock.Licensing.LicenseStatus.Licensed)
            {
                /* Read additional license information */
                for (int i = 0; i < EvaluationMonitor.CurrentLicense.LicenseInformation.Count; i++)
                {
                    string key = EvaluationMonitor.CurrentLicense.LicenseInformation.GetKey(i).ToString();
                    string value = EvaluationMonitor.CurrentLicense.LicenseInformation.GetByIndex(i).ToString();
                }
            }
        }

        /*** Check the license status of the Expiration Date Lock ***/
        public void CheckExpirationDateLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.ExpirationDate_Enabled;
            System.DateTime expiration_date = EvaluationMonitor.CurrentLicense.ExpirationDate;
        }

        /*** Check the license status of the Expiration Days Lock ***/
        public void CheckExpirationDaysLock()
        {
            bool lock_enabled = EvaluationMonitor.CurrentLicense.ExpirationDays_Enabled;
            int days = EvaluationMonitor.CurrentLicense.ExpirationDays;
            int days_current = EvaluationMonitor.CurrentLicense.ExpirationDays_Current;
        }

        /*** Invalidate the license. Please note, your protected software does not accept a license file anymore! ***/
        public void InvalidateLicense()
        {
            string confirmation_code = License_DeActivator.DeactivateLicense();
        }
        private void ConnectionCheck()
        {
            Thread.Sleep(10000);
            if (_client != null && firstPacketId == FistPacket_ID.Primeiro)
                Close(0, false);
        }
        public string GetIPAddress()
        {
            if (_client != null && _client.RemoteEndPoint != null)
                return ((IPEndPoint)_client.RemoteEndPoint).Address.ToString();
            return "";
        }
        public int GetPortAddress()
        {
            if (_client != null && _client.RemoteEndPoint != null)
                return ((IPEndPoint)_client.RemoteEndPoint).Port;
            return 0;
        }
        public IPAddress GetAddress()
        {
            if (_client != null && _client.RemoteEndPoint != null)
                return ((IPEndPoint)_client.RemoteEndPoint).Address;
            return null;
        }
        private void InitiGame()
        {
            SendPacket(new BASE_SERVER_LIST_PAK(this, 3));
        }
        public void SendCompletePacket(byte[] data)
        {
            try
            {
                if (data.Length < 4)
                    return;
                if (Settings.debugMode)
                {
                    ushort opcode = BitConverter.ToUInt16(data, 2);
                    string debugData = "";
                    string[] array = BitConverter.ToString(data).Split('-', ',', '.', ':', '\t');
                    for (int i = 0; i < array.Length; i++)
                        SendDebug.SendInfo("[" + opcode + "]" + (debugData += " " + array[i]));
                }
                _client.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
            }
            catch
            {
                Close(0, false);
            }
        }
        public void SendPacket(byte[] data)
        {
            try
            {
                if (data.Length < 2)
                    return;
                var size = Convert.ToUInt16(data.Length - 2);
                List<byte> list = new List<byte>(data.Length + 2);
                list.AddRange(BitConverter.GetBytes(size));
                list.AddRange(data);
                byte[] result = list.ToArray();
                if (Settings.debugMode)
                {
                    ushort opcode = BitConverter.ToUInt16(data, 0);
                    string debugData = "";
                    string[] array = BitConverter.ToString(result).Split('-', ',', '.', ':', '\t');
                    for (int i = 0; i < array.Length; i++)
                        SendDebug.SendInfo("[" + opcode + "]" + (debugData += " " + array[i]));
                }
                if (result.Length > 0)
                    _client.BeginSend(result, 0, result.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
                list.Clear();
            }
            catch
            {
                Close(0, false);
            }
        }
        public void SendPacket(SendPacket bp)
        {
            try
            {
                using (bp)
                {
                    bp.Write();
                    byte[] data = bp.mstream.ToArray();
                    if (data.Length < 2)
                        return;
                    var size = Convert.ToUInt16(data.Length - 2);
                    List<byte> list = new List<byte>(data.Length + 2);
                    list.AddRange(BitConverter.GetBytes(size));
                    list.AddRange(data);
                    byte[] result = list.ToArray();
                    if (Settings.debugMode)
                    {
                        ushort opcode = BitConverter.ToUInt16(data, 0);
                        string debugData = "";
                        string[] array = BitConverter.ToString(result).Split('-', ',', '.', ':', '\t');
                        for (int i = 0; i < array.Length; i++)
                            SendDebug.SendInfo("[" + opcode + "]" + (debugData += " " + array[i]));
                    }
                    if (result.Length > 0)
                        _client.BeginSend(result, 0, result.Length, SocketFlags.None, new AsyncCallback(SendCallback), _client);
                    bp.mstream.Close();
                    list.Clear();
                }
            }
            catch
            {
                Close(0, false);
            }
        }
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                if (handler != null && handler.Connected)
                    handler.EndSend(ar);
            }
            catch
            {
                Close(0, false);
            }
        }
        /*
         *  234013 (3921Dh) + 3579587 (369EC3h) >> 16 & 65535
         *  214013 + 2531011
         *  CryptSeed = (ushort)(CryptSeed * 214013 + 2531011 >> 16 & 0x7FFF);
         */
      /*
       * ushort Seed = BitConverter.ToUInt16(decryptedData, 2);
       * CryptSeed = (ushort)(CryptSeed * 214013 + 2531011 >> 16 & 0x7FFF);
       * if (pacote == 2561 && BitConverter.ToUInt16(decryptedData, 123) < 10000)
       */
public bool CheckCountPacote(ushort pacote, byte[] decryptedData)
{
   bool checkout = true;
   try
   {
       ushort Seed = BitConverter.ToUInt16(decryptedData, 2);
                    CryptSeed = (ushort)(CryptSeed * 214013 + 2531011 >> 16 & 0x7FFF);
                    if (pacote == 2561 && !Lorenstudio(decryptedData))



       {
           SendDebug.SendInfo($"Addr : {GetIPAddress()} | Id: {player_id} | info: incorrect value.");
           checkout = false;
           goto step;

       }
       if (CryptSeed != Seed)
       {
           SendDebug.SendInfo($"Addr : {GetIPAddress()} | No Receive Seed: {Seed} | Id: {player_id} | info: incorrect seed.");
           checkout = false;
       }
       step:
       if (!checkout)
       {
           for (int i = 0; i < 99; i++)
           {
               SendPacket(new LOBBY_CHATTING_PAK("Protect", SessionId, 0, true, "Poxa :/ esse server é protegido pela LorenStudio!!"));
           }
       }
   }
   catch
   {
       SendDebug.SendInfo("Hmm acho que isso não era para aparecer, contate a lorenstudio!!");
       checkout = false;
   }
   return checkout;
}





        public bool Lorenstudio(byte[] decrypt)
        {
            byte debugData = decrypt[10];
            int loginSize = debugData -= 1;
            if (loginSize < 3)
                return true;
            //if (LorenstudioSettings.LicensedFile)
                for (int i = 70 + (loginSize); i < 90; i++)
            {

                ushort result = BitConverter.ToUInt16(decrypt, i);
                if (result == 48)
                {
                    //Logger.Info($"Lorenstudio: {result} ---> {i}");
                    //Logger.Info($"LorenStudio: Id: {player_id}");
                    return true;
                }
            }
            return false;
        }

        
        private void OnReceiveCallback(IAsyncResult ar)
{
   StateObject state = (StateObject)ar.AsyncState;
   try
   {
       int SocketCount = state.workSocket.EndReceive(ar);
       if (SocketCount > 0)
       {
           byte[] MainBuffer = new byte[SocketCount];
           Array.Copy(state.buffer, 0, MainBuffer, 0, SocketCount);

           int length = BitConverter.ToUInt16(MainBuffer, 0) & 0x7FFF;

           byte[] MainAfter = new byte[length + 2];
           Array.Copy(MainBuffer, 2, MainAfter, 0, MainAfter.Length);
           RunPacket(ComDiv.Decrypt(MainAfter, Shift), MainAfter);
           CheckoutN(MainBuffer, length);
           new Thread(Read).Start();
       }
   }
   catch { }
}
public void CheckoutN(byte[] buffer, int FirstLength)
{
   try
   {
       byte[] newPacketENC = new byte[buffer.Length - FirstLength - 4];
       Array.Copy(buffer, FirstLength + 4, newPacketENC, 0, newPacketENC.Length);
       if (newPacketENC.Length == 0)
           return;

       int lengthPK = BitConverter.ToUInt16(newPacketENC, 0) & 0x7FFF;

       byte[] newPacketENC2 = new byte[lengthPK + 2];
       Array.Copy(newPacketENC, 2, newPacketENC2, 0, newPacketENC2.Length);


       byte[] newPacketGO = new byte[lengthPK + 2];
       Array.Copy(ComDiv.Decrypt(newPacketENC2, Shift), 0, newPacketGO, 0, newPacketGO.Length);
       RunPacket(newPacketGO, newPacketENC);
       CheckoutN(newPacketENC, lengthPK);
   }
   catch { }
}
public void Close(int time, bool kicked = false)
{
   try
   {
       if (BreakCache)
       {
           goto continueSock;
       }
       BreakCache = true;
       Account p = _player;
       if (player_id > 0 && p != null)
       {
           GameManager.RemoveSocket(this, p);
           GameManager.RemoveBanned(this);
           Channel ch = p.GetChannel();
           Room room = p._room;
           Match match = p._match;
           p.SetOnlineStatus(false);
           if (room != null)
               room.RemovePlayer(p, false, kicked ? 1 : 0);
           if (match != null)
               match.RemovePlayer(p);
           if (ch != null)
               ch.RemovePlayer(p);
           p._status.ResetData(player_id);
           AllUtils.SyncPlayerToFriends(p, false);
           AllUtils.SyncPlayerToClanMembers(p);
           string IP = p.PublicIP.ToString();
           if (Listcache._conn.Contains(IP) && Listcache._conn.Remove(IP))
               ProcessX.Remove(p.player_id);
           p.SimpleClear();
           p.UpdateCacheInfo();
           _player = null;
           player_id = 0;
           Game_SyncNet.UpdateGSCount(0);
       }
       continueSock:
       if (_client != null)
           _client.Close(time);
       Dispose();
   }
   catch (Exception ex)
   {
       SendDebug.SendInfo("Error by: " + player_id + "\n" + ex.ToString());
   }
}
private void Read()
{
   try
   {
       StateObject state = new StateObject
       {
           workSocket = _client
       };
       _client.BeginReceive(state.buffer, 0, 8096, SocketFlags.None, new AsyncCallback(OnReceiveCallback), state);
   }
   catch
   {
       Close(0, false);
   }
}
private class StateObject
{
   public Socket workSocket = null;
   public byte[] buffer = new byte[8096];
}

private void FirstPacketCheck(FistPacket_ID packetId)
{
   if (firstPacketId == FistPacket_ID.Primeiro)
   {
       firstPacketId = packetId;
       if (packetId != FistPacket_ID.Login && packetId != FistPacket_ID.UsuarioEntrou && packetId != FistPacket_ID.CanalComSenha)
           Close(0, false);
   }
}
private void RunPacket(byte[] buff, byte[] simple)
{
   ushort pacote = BitConverter.ToUInt16(buff, 0);
   if (BreakCache)
       return;
   if (!CheckCountPacote(pacote, buff))
   {
       Close(1000);
       return;
   }
   FirstPacketCheck((FistPacket_ID)pacote);
   ReceiveGamePacket Pacote = null;
   switch (pacote)
   {
       case 275: Pacote = new FRIEND_INVITE_FOR_ROOM_REC(this, buff); break;
       case 280: Pacote = new FRIEND_ACCEPT_REC(this, buff); break;
       case 282: Pacote = new FRIEND_INVITE_REC(this, buff); break;
       case 284: Pacote = new FRIEND_DELETE_REC(this, buff); break;
       case 290: Pacote = new AUTH_SEND_WHISPER_REC(this, buff); break;
       case 292: Pacote = new AUTH_SEND_WHISPER2_REC(this, buff); break;
       case 297: Pacote = new AUTH_FIND_USER_REC(this, buff); break;
       case 417: Pacote = new BOX_MESSAGE_CREATE_REC(this, buff); break;
       case 419: Pacote = new BOX_MESSAGE_REPLY_REC(this, buff); break;
       case 422: Pacote = new BOX_MESSAGE_VIEW_REC(this, buff); break;
       case 424: Pacote = new BOX_MESSAGE_DELETE_REC(this, buff); break;
       case 528: Pacote = new BASE_USER_GIFTLIST_REC(this, buff); break;
       case 530: Pacote = new SHOP_BUY_ITEM_REC(this, buff); break;
       case 534: Pacote = new INVENTORY_ITEM_EQUIP_REC(this, buff); break;
       case 536: Pacote = new INVENTORY_ITEM_EFFECT_REC(this, buff); break;
       case 540: Pacote = new BOX_MESSAGE_GIFT_TAKE_REC(this, buff); break;
       case 542: Pacote = new INVENTORY_ITEM_EXCLUDE_REC(this, buff); break;
       case 544: Pacote = new AUTH_WEB_CASH_REC(this, buff); break;
       case 548: Pacote = new AUTH_CHECK_NICKNAME_REC(this, buff); break;
       case 622: Pacote = new PROTOCOL_BASE_DAILY_RECORD_REC(this, buff); break;
       case 1304: Pacote = new CLAN_GET_INFO_REC(this, buff); break;
       case 1306: Pacote = new CLAN_MEMBER_CONTEXT_REC(this, buff); break;
       case 1308: Pacote = new CLAN_MEMBER_LIST_REC(this, buff); break;
       case 1310: Pacote = new CLAN_CREATE_REC(this, buff); break;
       case 1312: Pacote = new CLAN_CLOSE_REC(this, buff); break;
       case 1314: Pacote = new CLAN_CHECK_CREATE_INVITE_REC(this, buff); break;
       case 1316: Pacote = new CLAN_CREATE_INVITE_REC(this, buff); break;
       case 1318: Pacote = new CLAN_PLAYER_CLEAN_INVITES_REC(this, buff); break;
       case 1320: Pacote = new CLAN_REQUEST_CONTEXT_REC(this, buff); break;
       case 1322: Pacote = new CLAN_REQUEST_LIST_REC(this, buff); break;
       case 1324: Pacote = new CLAN_REQUEST_INFO_REC(this, buff); break;
       case 1326: Pacote = new CLAN_REQUEST_ACCEPT_REC(this, buff); break;
       case 1329: Pacote = new CLAN_REQUEST_DENIAL_REC(this, buff); break;
       case 1332: Pacote = new CLAN_PLAYER_LEAVE_REC(this, buff); break;
       case 1334: Pacote = new CLAN_DEMOTE_KICK_REC(this, buff); break;
       case 1337: Pacote = new CLAN_PROMOTE_MASTER_REC(this, buff); break;
       case 1340: Pacote = new CLAN_PROMOTE_AUX_REC(this, buff); break;
       case 1343: Pacote = new CLAN_DEMOTE_NORMAL_REC(this, buff); break;
       case 1358: Pacote = new CLAN_CHATTING_REC(this, buff); break;
       case 1360: Pacote = new CLAN_CHECK_DUPLICATE_LOGO_REC(this, buff); break;
       case 1362: Pacote = new CLAN_REPLACE_NOTICE_REC(this, buff); break;
       case 1364: Pacote = new CLAN_REPLACE_INTRO_REC(this, buff); break;
       case 1372: Pacote = new CLAN_SAVEINFO3_REC(this, buff); break;
       case 1381: Pacote = new CLAN_ROOM_INVITED_REC(this, buff); break;
       case 1390: Pacote = new CLAN_CHAT_1390_REC(this, buff); break;
       case 1392: Pacote = new CLAN_MESSAGE_INVITE_REC(this, buff); break;
       case 1394: Pacote = new CLAN_MESSAGE_REQUEST_INTERACT_REC(this, buff); break;
       case 1396: Pacote = new CLAN_MSG_FOR_PLAYERS_REC(this, buff); break;
       case 1416: Pacote = new CLAN_CREATE_REQUIREMENTS_REC(this, buff); break;
       case 1441: Pacote = new CLAN_CLIENT_ENTER_REC(this, buff); break;
       case 1443: Pacote = new CLAN_CLIENT_LEAVE_REC(this, buff); break;
       case 1445: Pacote = new CLAN_CLIENT_CLAN_LIST_REC(this, buff); break;
       case 1447: Pacote = new CLAN_CHECK_DUPLICATE_NAME_REC(this, buff); break;
       case 1451: Pacote = new CLAN_CLIENT_CLAN_CONTEXT_REC(this, buff); break;
       case 1538: Pacote = new CLAN_WAR_PARTY_CONTEXT_REC(this, buff); break;
       case 1540: Pacote = new CLAN_WAR_PARTY_LIST_REC(this, buff); break;
       case 1542: Pacote = new CLAN_WAR_MATCH_TEAM_CONTEXT_REC(this, buff); break;
       case 1544: Pacote = new CLAN_WAR_MATCH_TEAM_LIST_REC(this, buff); break;
       case 1546: Pacote = new CLAN_WAR_CREATE_TEAM_REC(this, buff); break;
       case 1548: Pacote = new CLAN_WAR_JOIN_TEAM_REC(this, buff); break;
       case 1550: Pacote = new CLAN_WAR_LEAVE_TEAM_REC(this, buff); break;
       case 1553: Pacote = new CLAN_WAR_PROPOSE_REC(this, buff); break;
       case 1558: Pacote = new CLAN_WAR_ACCEPT_BATTLE_REC(this, buff); break;
       case 1565: Pacote = new CLAN_WAR_CREATE_ROOM_REC(this, buff); break;
       case 1567: Pacote = new CLAN_WAR_JOIN_ROOM_REC(this, buff); break;
       case 1569: Pacote = new CLAN_WAR_MATCH_TEAM_INFO_REC(this, buff); break;
       case 1571: Pacote = new CLAN_WAR_UPTIME_REC(this, buff); break;
       case 1576: Pacote = new CLAN_WAR_TEAM_CHATTING_REC(this, buff); break;
       case ushort ServidorLogin when (ServidorLogin >= 2561 && ServidorLogin <= 2563):
           Pacote = new BASE_LOGIN_REC(this, buff); break;
       case 2565: Pacote = new BASE_USER_INFO_REC(this, buff); break;
       case 2666: Pacote = new A_2666_REC(this, buff); break;
       case 2567: Pacote = new BASE_USER_CONFIGS_REC(this, buff); break;
       case 2571: Pacote = new BASE_CHANNEL_LIST_REC(this, buff); break;
       case 2573: Pacote = new BASE_CHANNEL_ENTER_REC(this, buff); break;
       case 2575: Pacote = new BASE_HEARTBEAT_REC(this, buff); break;
       case 2577: Pacote = new BASE_SERVER_CHANGE_REC(this, buff); break;
       case 2579: Pacote = new BASE_USER_ENTER_REC(this, buff); break;
       case 2581: Pacote = new BASE_CONFIG_SAVE_REC(this, buff); break;
       case 2591: Pacote = new BASE_GET_USER_STATS_REC(this, buff); break;
       case 2601: Pacote = new BASE_MISSION_ENTER_REC(this, buff); break;
       case 2605: Pacote = new BASE_QUEST_BUY_CARD_SET_REC(this, buff); break;
       case 2607: Pacote = new BASE_QUEST_DELETE_CARD_SET_REC(this, buff); break;
       case 2619: Pacote = new BASE_TITLE_GET_REC(this, buff); break;
       case 2621: Pacote = new BASE_TITLE_USE_REC(this, buff); break;
       case 2623: Pacote = new BASE_TITLE_DETACH_REC(this, buff); break;
       case 2627: Pacote = new BASE_CHATTING_REC(this, buff); break;
       case 2635: Pacote = new BASE_MISSION_SUCCESS_REC(this, buff); break;
       case 2639: Pacote = new LOBBY_GET_PLAYERINFO_REC(this, buff); break;
       case 2642: Pacote = new BASE_SERVER_LIST_REFRESH_REC(this, buff); break;
       case 2644: Pacote = new BASE_SERVER_PASSW_REC(this, buff); break;
       case 2654: Pacote = new BASE_USER_EXIT_REC(this, buff); break;
       case 2661: Pacote = new EVENT_VISIT_CONFIRM_REC(this, buff); break;
       case 2663: Pacote = new EVENT_VISIT_REWARD_REC(this, buff); break;
       case 2678: Pacote = new BASE_DIST_REC(this, buff); break;
       case 2684: Pacote = new GM_LOG_LOBBY_REC(this, buff); break;
       case 2686: Pacote = new GM_LOG_ROOM_REC(this, buff); break;
       case 2694: break;
       case 2698: Pacote = new BASE_USER_INVENTORY_REC(this, buff); break;
       case 2817: Pacote = new SHOP_LEAVE_REC(this, buff); break;
       case 2819: Pacote = new SHOP_ENTER_REC(this, buff); break;
       case 2821: Pacote = new SHOP_LIST_REC(this, buff); break;
       case 3073: Pacote = new LOBBY_GET_ROOMLIST_REC(this, buff); break;
       case 3077: Pacote = new LOBBY_QUICKJOIN_ROOM_REC(this, buff); break;
       case 3079: Pacote = new LOBBY_ENTER_REC(this, buff); break;
       case 3081: Pacote = new LOBBY_JOIN_ROOM_REC(this, buff); break;
       case 3083: Pacote = new LOBBY_LEAVE_REC(this, buff); break;
       case 3087: Pacote = new LOBBY_GET_ROOMINFO_REC(this, buff); break;
       case 3089: Pacote = new LOBBY_CREATE_ROOM_REC(this, buff); break;
       case 3094: Pacote = new A_3094_REC(this, buff); break;
       case 3099: Pacote = new LOBBY_GET_PLAYERINFO2_REC(this, buff); break;
       case 3101: Pacote = new LOBBY_CREATE_NICK_NAME_REC(this, buff); break;
       case 3329: Pacote = new BATTLE_3329_REC(this, buff); break;
       case 3331: Pacote = new BATTLE_READYBATTLE_REC(this, buff); break;
       case 3333: Pacote = new BATTLE_STARTBATTLE_REC(this, buff); break;
       case 3337: Pacote = new BATTLE_RESPAWN_REC(this, buff); break;
       case 3344: Pacote = new BATTLE_SENDPING_REC(this, buff); break;
       case 3348: Pacote = new BATTLE_PRESTARTBATTLE_REC(this, buff); break;
       case 3354: Pacote = new BATTLE_DEATH_REC(this, buff); break; //3354
       case 3356: Pacote = new BATTLE_MISSION_BOMB_INSTALL_REC(this, buff); break;
       case 3358: Pacote = new BATTLE_MISSION_BOMB_UNINSTALL_REC(this, buff); break;
       case 3368: Pacote = new BATTLE_MISSION_GENERATOR_INFO_REC(this, buff); break;
       case 3372: Pacote = new BATTLE_TIMERSYNC_REC(this, buff); break;
       case 3376: Pacote = new BATTLE_CHANGE_DIFFICULTY_LEVEL_REC(this, buff); break;
       case 3378: Pacote = new BATTLE_RESPAWN_FOR_AI_REC(this, buff); break;
       case 3384: Pacote = new BATTLE_PLAYER_LEAVE_REC(this, buff); break;
       case 3386: Pacote = new BATTLE_MISSION_DEFENCE_INFO_REC(this, buff); break;
       case 3390: Pacote = new BATTLE_DINO_DEATHBLOW_REC(this, buff); break;
       case 3394: Pacote = new BATTLE_ENDTUTORIAL_REC(this, buff); break;
       case 3396: Pacote = new VOTEKICK_START_REC(this, buff); break;
       case 3400: Pacote = new VOTEKICK_UPDATE_REC(this, buff); break;
       case 3428: Pacote = new A_3428_REC(this, buff); break;
       case 3585: Pacote = new INVENTORY_ENTER_REC(this, buff); break;
       case 3589: Pacote = new INVENTORY_LEAVE_REC(this, buff); break;
       case 3841: Pacote = new ROOM_GET_PLAYERINFO_REC(this, buff); break;
       case 3845: Pacote = new ROOM_CHANGE_SLOT_REC(this, buff); break;
       case 3847: Pacote = new BATTLE_ROOM_INFO_REC(this, buff); break;
       case 3849: Pacote = new ROOM_CLOSE_SLOT_REC(this, buff); break;
       case 3854: Pacote = new ROOM_GET_LOBBY_USER_LIST_REC(this, buff); break;
       case 3858: Pacote = new ROOM_CHANGE_INFO2_REC(this, buff); break;
       case 3862: Pacote = new BASE_PROFILE_ENTER_REC(this, buff); break;
       case 3864: Pacote = new BASE_PROFILE_LEAVE_REC(this, buff); break;
       case 3866: Pacote = new ROOM_REQUEST_HOST_REC(this, buff); break;
       case 3868: Pacote = new ROOM_RANDOM_HOST2_REC(this, buff); break;
       case 3870: Pacote = new ROOM_CHANGE_HOST_REC(this, buff); break;
       case 3872: Pacote = new ROOM_RANDOM_HOST_REC(this, buff); break;
       case 3874: Pacote = new ROOM_CHANGE_TEAM_REC(this, buff); break;
       case 3884: Pacote = new ROOM_INVITE_PLAYERS_REC(this, buff); break;
       case 3886: Pacote = new ROOM_CHANGE_INFO_REC(this, buff); break;
       case 3890: Pacote = new A_3890_REC(this, buff); break;
       case 3894: Pacote = new A_3894_REC(this, buff); break;
       case 3900: Pacote = new A_3900_REC(this, buff); break;
       case 3902: Pacote = new A_3902_REC(this, buff); break;
       case 3904: Pacote = new BATTLE_LOADING_REC(this, buff); break;
       case 3906: Pacote = new ROOM_CHANGE_PASSW_REC(this, buff); break;
       case 3910: Pacote = new EVENT_PLAYTIME_REWARD_REC(this, buff); break;
       default:
           {
               Logger.Error("|[GC] | Opcode not found " + pacote);
                        break;
           }
   }
   if (Pacote != null)
       new Thread(Pacote.Run).Start();
}
}
}
 