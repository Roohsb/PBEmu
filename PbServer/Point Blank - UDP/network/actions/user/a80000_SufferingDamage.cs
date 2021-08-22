using Battle.data.models;

namespace Battle.network.actions.user
{
    public class a80000_SufferingDamage
    {
        public static Struct ReadInfo(ActionModel ac, ReceivePacket p, bool genLog) =>
            new Struct
            { _hitEffects = p.readC(), _hitPart = p.readC()};
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(2);
        }
        public static void WriteInfo(SendPacket s, ActionModel ac, ReceivePacket p, bool genLog)
        {
            WriteInfo(s, ReadInfo(ac, p, genLog));
        }
        public static void WriteInfo(SendPacket s, Struct info)
        {
            s.WriteC(info._hitEffects);
            s.WriteC(info._hitPart);
        }
        public class Struct
        {
            public byte _hitEffects, _hitPart;
        }
    }
}