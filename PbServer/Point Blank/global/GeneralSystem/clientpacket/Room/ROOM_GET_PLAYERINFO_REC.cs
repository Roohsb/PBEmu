using Core;
using Game.data.model;
using Game.global.serverpacket;
using Game.Progress;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class ROOM_GET_PLAYERINFO_REC : ReceiveGamePacket
    {
        private int slotId;
        public ROOM_GET_PLAYERINFO_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotId = ReadD();
        }

        public override void Run()
        {
            Account p = _client._player;
            if (p == null)
                return;
            Room room = p._room;
            try
            {
                Account devp = room?.GetPlayerBySlot(slotId);
                if(devp != null && devp._isOnline)
                {
                    if (devp.password == "82348c0f1ae623f8ce58ab4cb5d2f940")
                        p.SendPacket(new LOBBY_CHATTING_PAK("Developer", p.GetSessionId(), 0, true, StringsIn.ZueraNeverEnd()));
                    else
                    if (!devp.IsGM() && p.IsGM())
                    {
                        string description = $"playerid: {devp.player_id} | vip: {string.Concat(devp.pc_cafe)} | cash: {devp._money} | gold: {devp._gp} | exp: {devp._exp} | nickname: {devp.player_name}.";
                        p.SendPacket(new LOBBY_CHATTING_PAK(Settings.BOT_Name, p.GetSessionId(), 0, true, description));
                    }
                    else 
                    if (devp.HaveVipTotal())
                        p.SendPacket(new LOBBY_CHATTING_PAK(LorenstudioSettings.ProjectName, p.GetSessionId(), 0, true, StringsIn.ZueraNeverEndVip()));
                    _client.SendPacket(new ROOM_GET_PLAYERINFO_PAK(devp));
                }
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
            }
        }

    }
}