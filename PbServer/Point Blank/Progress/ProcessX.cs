using Game.data.model;
using Game.Progress;
using NetFwTypeLib;
using System;
using System.Linq;

namespace Game
{
    public class ProcessX
    {
        public static INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        public static void Remove(long playerid)
        {
            try
            {
                INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == ("UDP- " + playerid)).FirstOrDefault();
                if (firewallRule != null)
                {
                    SendDebug.SendInfo("Successfully disconnected firewall. " + playerid);
                    firewallPolicy.Rules.Remove(firewallRule.Name);
                }
            }
            catch
            {
                Console.WriteLine("Error removing connection.");
            }
        }
        public static string BloquearVPS()
        {
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "se fode ai otario").FirstOrDefault();
            if (firewallRule == null)
            {
                firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Name = "se fode ai otario";
                firewallPolicy.Rules.Add(firewallRule);
                firewallRule.Description = ""; // DESCRIÇÃO
                firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL; // Todos os Perfil Publico ao privado
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY; //Tipo de Protocolo
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // ENTRADA E SAIDA
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; //BLOQUEAR OU LIBERAR PACOTES
                firewallRule.Enabled = true;//Ativar o rule
                firewallRule.RemoteAddresses = NextModel.IPAdress;
            }
            return "";
        }
        public static string BloquearAbuso(string IP)
        {
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "floood").FirstOrDefault();
            if (firewallRule == null)
            {
                firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Name = "floood";
                firewallPolicy.Rules.Add(firewallRule);
                firewallRule.Description = "Foi detectado um ataque e o bloqueio foi direto!"; // DESCRIÇÃO
                firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL; // Todos os Perfil Publico ao privado
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY; //Tipo de Protocolo
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // ENTRADA E SAIDA
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; //BLOQUEAR OU LIBERAR PACOTES
                firewallRule.Enabled = true;//Ativar o rule
                firewallRule.RemoteAddresses = IP;
            }
            else
            {
                firewallRule.RemoteAddresses = firewallRule.RemoteAddresses + "," + IP;
            }
            return "";
        }
    }
}
