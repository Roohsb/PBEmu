using Game.data.model;
using IpPublicKnowledge;
using System;
using System.Collections.Generic;
using System.Net;
using IntelliLock.Licensing;
namespace Game
{
    /*
     * KRrBwHs2wr5D Project Eros
     * ErS291H8A2b3 Point Blank Blaze
     * Ps2jd21bo911 Point Blank Second
     * Pk1n37s2nd21 Project Fireway
     * XPks70MsL1D0 Project Bloodi
     * P13ns19s7x13 Project Italy
     * D23s1XLx19S1 Project Mafia
     */
    public class NextModel
    {
        public static string IPAdress, NomeDoPB, Senha, BuildPB = "2005.13";

        public static int SomeMais1 = 3;

        public static int Pass = 02031999;
        public static string Pais(IPAddress addr)
        {
            try
            {
                IPI acs = IPK.GetIpInfo(addr);
               return " country: '" + acs.country + "', City: '" + acs.city + "', Region '" + acs.region + "'. ";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Error found, contact GM.";
            }
        }
        public static void IPPublic()
        {
            try
            {
                IPAdress = new WebClient().DownloadString("https://ipinfo.io/ip");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        /// <summary>
        /// 1 ProjectX, 
        /// </summary>
        public static void Servicos(int index)
        {
            switch (index)
            {
                case 1:
                    NomeDoPB = "FREEMODE";
                    Senha = "Sim";
                    break;
                case 2:
                    NomeDoPB = "PREMIUMMODE";
                    Senha = "fror+$5Imlf";
                    break;
                case 3:
                    NomeDoPB = "DevMode";
                    Senha = "f2or+$5Imlf";
                    break;
            }
        }
    }
}
