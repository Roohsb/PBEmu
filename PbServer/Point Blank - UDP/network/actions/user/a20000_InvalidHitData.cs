using Battle.data;
using Battle.data.enums;
using SharpDX;
using System.Collections.Generic;

namespace Battle.network.actions.user
{
    public class a20000_InvalidHitData
    {
        public class HitData
        {
            public ushort _hitInfo;
            public Half3 FirePos, HitPos;
            public HitType HitEnum;
        }
        public static List<HitData> ReadInfo(ReceivePacket p, bool genLog, bool OnlyBytes = false) => BaseReadInfo(p, OnlyBytes, genLog);
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(14 * p.readC());
        }
        private static List<HitData> BaseReadInfo(ReceivePacket p, bool OnlyBytes, bool genLog)
        {
            List<HitData> hits = new List<HitData>();
            int objsCount = p.readC();
            for (int ob = 0; ob < objsCount; ob++)
            {
                HitData hit = new HitData
                {
                    _hitInfo = p.readUH(),
                    FirePos = p.readUHVector(),
                    HitPos = p.readUHVector()
                };
                if (!OnlyBytes)
                    hit.HitEnum = (HitType)AllUtils.GetHitHelmet(hit._hitInfo);
                hits.Add(hit);
            }
            return hits;
        }
        public static void writeInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            WriteInfo(s, ReadInfo(p, genLog, true));
        }
        public static void WriteInfo(SendPacket s, List<HitData> hits)
        {
            s.WriteC((byte)hits.Count);
            for (int i = 0; i < hits.Count; i++)
            {
                HitData hit = hits[i];
                s.WriteH(hit._hitInfo);
                s.WriteHVector(hit.FirePos);
                s.WriteHVector(hit.HitPos);
            }
        }
    }
}