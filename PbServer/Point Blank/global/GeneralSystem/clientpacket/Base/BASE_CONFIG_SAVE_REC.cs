using Core.managers;
using Core.models.account.players;
using Core.server;
using Game.data.model;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_CONFIG_SAVE_REC : ReceiveGamePacket
    {
        private int type;
        private DBQuery query = new DBQuery();
        public BASE_CONFIG_SAVE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            Account p = _client._player;
            if (p == null)
                return;
            bool ConfigIsValid = p._config != null;
            if (!ConfigIsValid)
            {
                ConfigIsValid = PlayerManager.CreateConfigDB(p.player_id);
                if (ConfigIsValid)
                    p._config = new PlayerConfig();
            }
            if (!ConfigIsValid)
                return;
            PlayerConfig config = p._config;
            //0x10000000
            type = ReadD();
            if ((type & 1) == 1)
            {
                config.blood = ReadH();
                config.sight = ReadC();
                config.hand = ReadC();
                config.config = ReadD();
                config.audio_enable = ReadC();
                ReadB(5);
                config.audio1 = ReadC();
                config.audio2 = ReadC();
                config.fov = ReadH();
                config.sensibilidade = ReadC();
                config.mouse_invertido = ReadC();
                ReadB(2);
                config.msgConvite = ReadC();
                config.chatSussurro = ReadC();
                config.macro = ReadC();
                ReadB(3);
            }
            if ((type & 2) == 2)
            {
                ReadB(5);
                config.keys = ReadB(215);
            }
            if ((type & 4) == 4)
            {
                config.macro_1 = ReadS(ReadC());
                config.macro_2 = ReadS(ReadC());
                config.macro_3 = ReadS(ReadC());
                config.macro_4 = ReadS(ReadC());
                config.macro_5 = ReadS(ReadC());
            }
        }

        public override void Run()
        {
            Account p = _client._player;
            if (p == null)
                return;
            PlayerConfig config = p._config;
            if (config == null)
                return;
            if ((type & 1) == 1)
                PlayerManager.UpdateConfigs(query, config);
            if ((type & 2) == 2)
                query.AddQuery("keys", config.keys);
            if ((type & 4) == 4)
                PlayerManager.UpdateMacros(query, config, type);
            ComDiv.UpdateDB("player_configs", "owner_id", _client.player_id, query.GetTables(), query.GetValues());
        }
    }
}