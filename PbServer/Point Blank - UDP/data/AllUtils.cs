using Battle.data.enums;
using Battle.data.enums.weapon;
using System;

namespace Battle.data
{
    public class AllUtils
    {
        public static float GetDuration(DateTime date) => (float)((DateTime.Now - date).TotalSeconds);
        public static int CreateItemId(int class1, int usage, int classtype, int number) =>
            (class1 * 100000000) + (usage * 1000000) + (classtype * 1000) + number;
        public static int ItemClass(ClassType cw)
        {
            int valor = 1;
            switch (cw)
            {
                case ClassType.Assault: 
                    valor = 1; break;
                case ClassType classType when (classType == ClassType.SMG || classType == ClassType.DualSMG):
                    valor = 2; break;
                case ClassType.Sniper: 
                    valor = 3; break;
                case ClassType classType when (classType == ClassType.Shotgun || classType == ClassType.DualShotgun):
                    valor = 4; break;
                case ClassType.MG:
                    valor = 5; break;
                case ClassType classType when (classType == ClassType.HandGun || classType == ClassType.DualHandGun || classType == ClassType.CIC):
                    valor = 6; break;
                case ClassType classType when (classType == ClassType.Knife || classType == ClassType.DualKnife || classType == ClassType.Knuckle):
                    valor = 7; break;
                case ClassType.Throwing: 
                    valor = 8; break;
                case ClassType.Item: 
                    valor = 9; break;
                case ClassType.Dino: 
                    valor = 0; break;
            }
            return valor;
        }
        public static ObjectType GetHitType(uint info) => (ObjectType)(info & 3);
        public static int GetHitWho(uint info) => (int)((info >> 2) & 511);
        public static int GetHitPart(uint info) => (int)((info >> 11) & 63);
        public static ushort GetHitDamageBOT(uint info) => (ushort)(info >> 20);
        public static ushort GetHitDamageNORMAL(uint info) => (ushort)(info >> 21);
        public static int GetHitHelmet(uint info) => (int)((info >> 17) & 7);
        public static int GetRoomInfo(uint UniqueRoomId, int type)
        {
            switch (type)
            {
                case 0: return (int)(UniqueRoomId & 0xfff);
                case 1: return (int)((UniqueRoomId >> 12) & 0xff);
                case 2: return (int)((UniqueRoomId >> 20) & 0xfff);
                default:
                    {
                        return 0;
                    }
            }
        }
        public static byte[] Encrypt(byte[] data, int shift)
        {
            byte[] result = new byte[data.Length];
            Buffer.BlockCopy(data, 0, result, 0, result.Length);
            int length = result.Length;
            byte first = result[0];
            byte current;
            for (int i = 0; i < length; i++)
            {
                current = i >= (length - 1) ? first : result[i + 1];
                result[i] = (byte)(current >> (8 - shift) | (result[i] << shift));
            }
            return result;
        }
        public static byte[] Decrypt(byte[] data, int shift)
        {
            try
            {
                byte[] result = new byte[data.Length];
                Buffer.BlockCopy(data, 0, result, 0, result.Length);
                int length = result.Length;
                byte last = result[length - 1];
                byte current;
                for (int i = length - 1; (i & 0x80000000) == 0; i--)
                {
                    current = i <= 0 ? last : result[i - 1];
                    result[i] = (byte)(current << (8 - shift) | result[i] >> shift);
                }
                return result;
            }
            catch { Logger.Warning(BitConverter.ToString(data)); return new byte[0]; }
        }
        public static int Percentage(int total, int percent) => (total * percent / 100);
        public static float Percentage(float total, int percent) => (total * percent / 100);
    }
}