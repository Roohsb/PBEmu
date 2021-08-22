using Battle.config;
using Battle.network;
using NetFwTypeLib;
using System;
using System.Linq;

namespace Battle.data.sync.client_side
{
    public class GetPrestart
    {
        public static INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        public static string IP, Nick, Country;
        public static long player_id;
        public static void Conection(ReceivePacket p)
        {
            try
            {
                Nick = p.readS(p.readC());
                player_id = p.readQ();
                IP = p.readS(p.readC());
                Country = p.readS(p.readC());
                Accpted();
            }
            catch (Exception ex)
            {
                Logger.Warning("IP error: " + IP + " nick: " + Nick + " " + ex.ToString());
            }
        }
        public static void Bloqueando(string IP)
        {
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "Bloqueado para Battle.").FirstOrDefault();
            bool action = false;
            if (firewallRule == null)
            {
                firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Name = "Bloqueado para Battle.";
                firewallPolicy.Rules.Add(firewallRule);
                firewallRule.Description = ""; // DESCRIÇÃO
                firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL; // Todos os Perfil Publico ao privado
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_ANY; //Tipo de Protocolo
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // ENTRADA E SAIDA
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; //BLOQUEAR OU LIBERAR PACOTES
                firewallRule.Enabled = true;//Ativar o rule
                firewallRule.RemoteAddresses = IP;
                action = true;
            }
            else
            {
                firewallRule.RemoteAddresses = firewallRule.RemoteAddresses + "," + IP;
                action = true;
            }
            if (action)
            {
                Logger.Warning("[Battle] Blocked with IP added: " + IP);
            }
        }
        public static void Accpted()
        {
            INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name == "UDP- " + player_id).FirstOrDefault();
            bool action = false;
            if (firewallRule == null)
            {
                firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Name = "UDP- " + player_id;
                firewallPolicy.Rules.Add(firewallRule);
                firewallRule.Description = ""; // DESCRIÇÃO
                firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;// Todos os Perfil Publico ao privado
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP; //Tipo de Protocolo
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN; // ENTRADA E SAIDA
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW; //BLOQUEAR OU LIBERAR PACOTES
                firewallRule.Enabled = true; //Ativar o rule
                firewallRule.RemoteAddresses = IP;
                action = true;
            }
            else
            {
                firewallRule.RemoteAddresses = firewallRule.RemoteAddresses + "," + IP;
                action = true;
            }
            if (action && Config.HostLogger)
            {
                Logger.Warning("----------------------------------------------------------------------------");
                Logger.InBattle("[Battle] the player '" + Nick + "' was added: " + IP + ".");
                Logger.InBattle("[Battle] " + Country);
            }
        }
        public static void Remove()
        {
            int index = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name.Contains("UDP- ")).Count();
            for (int i = 0; i < index; ++i)
            {
                INetFwRule firewallRule = firewallPolicy.Rules.OfType<INetFwRule>().Where(x => x.Name.Contains("UDP- ")).FirstOrDefault();
                if (firewallRule != null)
                    firewallPolicy.Rules.Remove(firewallRule.Name);
            }
        }
    }
}
