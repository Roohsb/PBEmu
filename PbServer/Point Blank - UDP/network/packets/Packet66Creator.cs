
namespace Battle.network.packets
{
    public class Packet66Creator
    {
        /// <summary>
        /// Gera um código do protocolo 66.
        /// </summary>
        /// <returns></returns>
        public static byte[] GetCode66()
        {
            using (SendPacket s = new SendPacket())
            {
                s.WriteC(66);
                s.WriteC(0);
                s.WriteT(0);
                s.WriteC(0);
                s.WriteH(13);
                s.WriteD(0);
                return s.mstream.ToArray();
            }
        }
    }
}