using Core.server;
using Game.data.model;
using Game.data.utils;
using System.Collections.Generic;

namespace Game.global.serverpacket
{
    public class BATTLE_ROUND_RESTART_PAK : SendPacket
    {
        private Room _r;
        private List<int> _dinos;
        private bool isBotMode;
        public BATTLE_ROUND_RESTART_PAK(Room r, List<int> dinos, bool isBotMode)
        {
            _r = r;
            _dinos = dinos;
            this.isBotMode = isBotMode;
        }
        public BATTLE_ROUND_RESTART_PAK(Room r)
        {
            _r = r;
            _dinos = AllUtils.getDinossaurs(r, false, -1);
            isBotMode = _r.IsBotMode();
        }
        public override void Write()
        {
            WriteH(3351);
            WriteH(AllUtils.getSlotsFlag(_r, false, false));
            if (_r.room_type == 8)
                WriteB(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF });
            else if (isBotMode)
                WriteB(new byte[10]);
            else if (_r.room_type == 7 || _r.room_type == 12)
            {
                int TRex = _dinos.Count == 1 || _r.room_type == 12 ? 255 : _r.TRex;
                WriteC((byte)TRex); //T-Rex || 255 (não tem t-rex)
                for (int slotid = 0; slotid < _dinos.Count; slotid++)
                {
                    int slot = _dinos[slotid];
                    if (slot != _r.TRex && _r.room_type == 7 || _r.room_type == 12)
                        WriteC((byte)slot);
                }

                int falta = 8 - _dinos.Count - (TRex == 255 ? 1 : 0);
                for (int i = 0; i < falta; i++)
                    WriteC(255);
                WriteC(255);
                WriteC(255);
            }
            else
                WriteB(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255, 255, 255 });//writeB(new byte[] { 255, 1, 255, 255, 255, 255, 255, 255, 255, 255 });
        }
    }
}