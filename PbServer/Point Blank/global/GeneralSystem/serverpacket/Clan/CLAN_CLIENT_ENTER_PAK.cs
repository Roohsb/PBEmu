using Core.server;
using Game.data.managers;
using System;

namespace Game.global.serverpacket
{
    public class CLAN_CLIENT_ENTER_PAK : SendPacket
    {
        private int _type, _clanId;
        public CLAN_CLIENT_ENTER_PAK(int id, int access)
        {
            _clanId = id;
            _type = access;
        }

        public override void Write()
        {
            WriteH(1442);
            WriteD(_clanId);
            WriteD(_type);
            if (_clanId == 0 || _type == 0)
            {
                WriteD(ClanManager._clans.Count);
                WriteC(170);
                WriteH((ushort)Math.Ceiling(ClanManager._clans.Count / 170d));
                WriteD(uint.Parse(DateTime.Now.ToString("MMddHHmmss")));
            }
        }
    }
}