using Battle.config;
using Battle.data.enums.weapon;
using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a800_WeaponAmmo
    {
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
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
        public static void writeInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(ac, p, genLog);
            s.WriteC(info._weaponFlag);
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