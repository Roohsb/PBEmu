using System;
using System.Collections.Generic;

namespace Battle.network.actions.user
{
    public class a4000_BotHitData
    {
        public class HitData
        {
            public byte _weaponSlot;
            public ushort _weaponInfo, _eixoX, _eixoY, _eixoZ, _unk;
            public uint _hitInfo;
        }
        public static void ReadInfo(ReceivePacket p)
        {
            p.Advance(15 * p.readC());
        }
        public static List<HitData> ReadInfo(ReceivePacket p, bool genLog)
        {
            List<HitData> hits = new List<HitData>();
            int count = p.readC();
            for (int k = 0; k < count; k++)
            {
                HitData hit = new HitData
                {
                    _hitInfo = p.readUD(),
                    _weaponInfo = p.readUH(),
                    _weaponSlot = p.readC(),
                    _unk = p.readUH(),
                    _eixoX = p.readUH(),
                    _eixoY = p.readUH(),
                    _eixoZ = p.readUH()
                };
                if (genLog)
                {
                    Logger.Warning("P: " + hit._eixoX + ";" + hit._eixoY + ";" + hit._eixoZ);
                    Logger.Warning("[" + k + "] 16384: " + BitConverter.ToString(p.getBuffer()));
                }
                hits.Add(hit);
            }
            return hits;
        }
        public static void WriteInfo(SendPacket s, ReceivePacket p, bool genLog)
        {
            List<HitData> hits = ReadInfo(p, genLog);
            s.WriteC((byte)hits.Count);
            for (int i = 0; i < hits.Count; i++)
            {
                HitData hit = hits[i];
                s.WriteD(hit._hitInfo);
                s.WriteH(hit._weaponInfo);
                s.WriteC(hit._weaponSlot);
                s.WriteH(hit._unk);
                s.WriteH(hit._eixoX);
                s.WriteH(hit._eixoY);
                s.WriteH(hit._eixoZ);
            }
        }
    }
}