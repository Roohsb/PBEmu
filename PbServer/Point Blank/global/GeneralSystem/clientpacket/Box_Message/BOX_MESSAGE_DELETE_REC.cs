﻿using Core;
using Core.managers;
using Game.global.serverpacket;
using System;
using System.Collections.Generic;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BOX_MESSAGE_DELETE_REC : ReceiveGamePacket
    {
        private uint erro;
        private List<object> objs = new List<object>();
        public BOX_MESSAGE_DELETE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            int count = ReadC();
            for (int i = 0; i < count; i++)
            {
                objs.Add(ReadD());
            }
        }

        public override void Run()
        {
            if (_client._player == null)
                return;
            try
            {
                if (!MessageManager.DeleteMessages(objs, _client.player_id))
                    erro = 0x80000000;
                _client.SendPacket(new BOX_MESSAGE_DELETE_PAK(erro, objs));
                objs = null;
            }
            catch (Exception ex)
            {
                Logger.Info("[BOX_MESSAGE_DELETE_REC] " + ex.ToString());
            }
        }
    }
}