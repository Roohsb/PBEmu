namespace Battle.network.actions.user
{
    public class a4_PositionSync
    {
        public class Struct
        {
            public ushort _rotationX, _rotationY, _rotationZ, _cameraX, _cameraY, _area;
        }
        public static Struct ReadInfo(ReceivePacket p, bool genLog) =>
            new Struct
            {
                _rotationX = p.readUH(),
                _rotationY = p.readUH(),
                _rotationZ = p.readUH(),
                _cameraX = p.readUH(),
                _cameraY = p.readUH(),
                _area = p.readUH()
            };
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            WriteInfo(s, ReadInfo(p, genLog));
        }
        public static void WriteInfo(SendPacket s, Struct info)
        {
            s.WriteH(info._rotationX);
            s.WriteH(info._rotationY);
            s.WriteH(info._rotationZ);
            s.WriteH(info._cameraX);
            s.WriteH(info._cameraY);
            s.WriteH(info._area);
        }
    }
}