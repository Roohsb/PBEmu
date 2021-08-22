using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a80_WeaponRecoil
    {
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
             new Struct
             {
                 _RecoilHorzAngle = p.readT(),
                 _RecoilHorzMax = p.readT(),
                 _RecoilVertAngle = p.readT(),
                 _RecoilVertMax = p.readT(),
                 _Deviation = p.readT(),
                 _weaponId = p.readUH(), //weaponId / 64
                 _weaponSlot = p.readC(), //weaponSlot
                 _unkV = p.readC(), //ping?
                 _RecoilHorzCount = p.readC()
             };
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(25);
        }
        public static void writeInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(ac, p, genLog);
            s.WriteT(info._RecoilHorzAngle);
            s.WriteT(info._RecoilHorzMax);
            s.WriteT(info._RecoilVertAngle);
            s.WriteT(info._RecoilVertMax);
            s.WriteT(info._Deviation);
            s.WriteH(info._weaponId);
            s.WriteC(info._weaponSlot);
            s.WriteC(info._unkV);
            s.WriteC(info._RecoilHorzCount);
        }
        public class Struct
        {
            public float _RecoilHorzAngle, _RecoilHorzMax, _RecoilVertAngle, _RecoilVertMax, _Deviation;
            public ushort _weaponId;
            public byte _weaponSlot, _unkV, _RecoilHorzCount;
        }
    }
}