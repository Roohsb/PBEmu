using Core;
using Core.managers;
using Core.models.account.title;
using Core.xml;
using Game.data.model;
using Game.global.serverpacket;
using System;

namespace Game.global.GeneralSystem.clientpacket
{
    public class BASE_TITLE_USE_REC : ReceiveGamePacket
    {
        private byte slotIdx, titleId;
        private uint erro;
        public BASE_TITLE_USE_REC(GameClient client, byte[] data)
        {
            Inicial(client, data);
        }

        public override void Read()
        {
            slotIdx = ReadC();
            titleId = ReadC();
        }

        public override void Run()
        {
            try
            {
                Account p = _client._player;
                if (p == null)
                    return;
                PlayerTitles t = p._titles;
                TitleQ titleQ = TitlesXML.getTitle(titleId);
                TitlesXML.get3Titles(t.Equiped1, t.Equiped2, t.Equiped3, out TitleQ eq1, out TitleQ eq2, out TitleQ eq3, false);
                if (slotIdx >= 3 || titleId >= 45 || t == null || titleQ == null || titleQ._classId == eq1._classId && slotIdx != 0 || titleQ._classId == eq2._classId && slotIdx != 1 || titleQ._classId == eq3._classId && slotIdx != 2 || !t.Contains(titleQ._flag) || t.Equiped1 == titleId || t.Equiped2 == titleId || t.Equiped3 == titleId)
                    erro = 0x80000000;
                else
                {
                    if (TitleManager.GetInstance().UpdateEquipedTitle(t.ownerId, slotIdx, titleId))
                        t.SetEquip(slotIdx, titleId);
                    else
                        erro = 0x80000000;
                }
                _client.SendPacket(new BASE_TITLE_USE_PAK(erro));
            }
            catch (Exception ex)
            {
                Logger.Info("BASE_TITLE_USE_REC: " + ex.ToString());
            }
        }
    }
}