namespace Battle.network.actions.others
{
    public class Code2_WeaponSync
    {
        public class Struct
        {
            public byte _weaponFlag;
            public ushort _posX, _posY, _posZ, _unk4, _unk5, _unk6, _unk7;
        }
        public static byte[] ReadInfo(ReceivePacket p) => p.readB(15);
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
            new Struct
            {
                _weaponFlag = p.readC(),
                _posX = p.readUH(),
                _posY = p.readUH(),
                _posZ = p.readUH(),
                _unk4 = p.readUH(),
                _unk5 = p.readUH(),
                _unk6 = p.readUH(),
                _unk7 = p.readUH()
            };
        public static void WriteInfo(SendPacket s, ReceivePacket p)
        {
            s.WriteB(ReadInfo(p));
        }
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            Struct info = ReadInfo(p, genLog);
            s.WriteC(info._weaponFlag);
            s.WriteH(info._posX);
            s.WriteH(info._posY);
            s.WriteH(info._posZ);
            s.WriteH(info._unk4);
            s.WriteH(info._unk5);
            s.WriteH(info._unk6);
            s.WriteH(info._unk7);
        }
    }
}