using Battle.config;
using System;

namespace Battle.network.actions.others
{
    public class Code1_GrenadeSync
    {
        public class Struct
        {
            public int WeaponNumber, WeaponClass;
            public ushort _weaponInfo, _objPos_x, _objPos_y, _objPos_z, _unk, _unk5, _unk6, _unk7, _grenadesCount;
            public byte _weaponSlot;
            public byte[] _unk8;
        }
        public static Struct ReadInfo(ReceivePacket p, bool genLog, bool OnlyBytes = false) => BaseReadInfo(p, OnlyBytes, genLog);
        private static Struct BaseReadInfo(ReceivePacket p, bool OnlyBytes, bool genLog)
        {
            Struct info = new Struct
            {
                _weaponInfo = p.readUH(),
                _weaponSlot = p.readC(),
                _unk = p.readUH(),
                _objPos_x = p.readUH(),
                _objPos_y = p.readUH(),
                _objPos_z = p.readUH(),
                _unk5 = p.readUH(),
                _unk6 = p.readUH(),
                _unk7 = p.readUH(),
                _grenadesCount = p.readUH()

            };
            info._unk8 = Config.ServerVersion == "1.15.42" ? p.readB(6) : null;
            if (!OnlyBytes)
            {
                info.WeaponNumber = (info._weaponInfo >> 6);
                info.WeaponClass = (info._weaponInfo & 47);
            }
            if (genLog)
            {
                Logger.Warning("[code1_GrenadeSync] " + BitConverter.ToString(p.getBuffer()));
                Logger.Warning("[code1_GrenadeSync] wInfo: " + info._weaponInfo + "; wSlot: " + info._weaponSlot + "; u: " + info._unk + "; obpX: " + info._objPos_x + "; obpY: " + info._objPos_y + "; obpZ: " + info._objPos_z + "; u5: " + info._unk5 + "; u6: " + info._unk6 + "; u7: " + info._unk7 + "; u8: " + info._unk8);
            }
            return info;
        }
        public static byte[] ReadInfo(ReceivePacket p) => Config.ServerVersion == "1.15.37" ? p.readB(19) : p.readB(25);
        public static void WriteInfo(SendPacket s, ReceivePacket p)
        {
            s.WriteB(ReadInfo(p));
        }
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(p, genLog, true);
            s.WriteH(info._weaponInfo);
            s.WriteC(info._weaponSlot);
            s.WriteH(info._unk);
            s.WriteH(info._objPos_x);
            s.WriteH(info._objPos_y);
            s.WriteH(info._objPos_z);
            s.WriteH(info._unk5);
            s.WriteH(info._unk6);
            s.WriteH(info._unk7);
            s.WriteH(info._grenadesCount);
            s.WriteB(Config.ServerVersion == "1.15.42" ? info._unk8 : null);
        }
    }
}