using Game.data.model;
using IpPublicKnowledge;
using System;
using System.Collections.Generic;
using System.Net;
namespace Game
{

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
                    NomeDoPB = "LIGARSERVER";
                    Senha = "Sim";
                    break;
            }
        }
    }
}
