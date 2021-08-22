using Battle.config;
using Battle.data.enums.weapon;
using System;

namespace Battle.network.actions.user
{
    public class a1000_DropWeapon
    {
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
            new Struct
            {
                _weaponFlag = p.readC(),
                _weaponClass = p.readC(),
                _weaponId = p.readUH(),
                _ammoPrin = p.readC(),
                _ammoDual = p.readC(),
                _ammoTotal = p.readUH()
            };
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(8);
        }
        public static void writeInfo(SendPacket s, ReceivePacket p, bool genLog, int count)
        {
            Struct info = ReadInfo(p, genLog);
            s.WriteC((byte)(info._weaponFlag + count));
            s.WriteC(info._weaponClass);
            s.WriteH(info._weaponId);
            if (Config.useMaxAmmoInDrop)
            {
                s.WriteC(255);
                s.WriteC(info._ammoDual);
                s.WriteH(10000);
            }
            else
            {
                s.WriteC(info._ammoPrin);
                s.WriteC(info._ammoDual);
                s.WriteH(info._ammoTotal);
            }
        }
        public class Struct
        {
            public byte _weaponFlag, _weaponClass, _ammoPrin, _ammoDual;
            public ushort _weaponId, _ammoTotal;
        }
    }
}