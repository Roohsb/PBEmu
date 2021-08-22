﻿using Core;
using System;
using System.Text;

namespace Game.global
{
    public abstract class ReceiveGamePacket
    {
        private byte[] _buffer;
        private int _offset = 4;
        public GameClient _client;
        protected internal void Inicial(GameClient client, byte[] buffer)
        {
            _client = client;
            _buffer = buffer;
            Read();
        }

        protected internal int ReadD()
        {
            int num = BitConverter.ToInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        protected internal uint ReadUD()
        {
            uint num = BitConverter.ToUInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        protected internal byte ReadC() => _buffer[_offset++];
        protected internal byte[] ReadB(int Length)
        {
            byte[] result = new byte[Length];
            Array.Copy(_buffer, _offset, result, 0, Length);
            _offset += Length;
            return result;
        }
        protected internal short ReadH()
        {
            short num = BitConverter.ToInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }
        protected internal ushort ReadUH()
        {
            ushort num = BitConverter.ToUInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }
        protected internal double ReadF()
        {
            double num = BitConverter.ToDouble(_buffer, _offset);
            _offset += 8;
            return num;
        }
        protected internal float ReadT()
        {
            float num = BitConverter.ToSingle(_buffer, _offset);
            _offset += 4;
            return num;
        }
        protected internal long ReadQ()
        {
            long num = BitConverter.ToInt64(_buffer, _offset);
            _offset += 8;
            return num;
        }
        protected internal ulong ReadUQ()
        {
            ulong num = BitConverter.ToUInt64(_buffer, _offset);
            _offset += 8;
            return num;
        }
        protected internal string ReadS(int Length)
        {
            string str = "";
            try
            {
                str = ConfigGB.EncodeText.GetString(_buffer, _offset, Length);
                int length = str.IndexOf(char.MinValue);
                if (length != -1)
                    str = str.Substring(0, length);
                _offset += Length;
            }
            catch
            {
            }
            return str;
        }
        protected internal string ReadS(int Length, int CodePage)
        {
            string str = "";
            try
            {
                str = Encoding.GetEncoding(CodePage).GetString(_buffer, _offset, Length);
                int length = str.IndexOf(char.MinValue);
                if (length != -1)
                    str = str.Substring(0, length);
                _offset += Length;
            }
            catch
            {
            }
            return str;
        }
        /// <summary>
        /// Usa Unicode.
        /// </summary>
        /// <returns></returns>
        protected internal string ReadS()
        {
            string result = "";
            try
            {
                int count = (_buffer.Length - _offset);
                result = Encoding.Unicode.GetString(_buffer, _offset, count);
                int idx = result.IndexOf(char.MinValue);
                if (idx != -1)
                    result = result.Substring(0, idx);
                _offset += result.Length + 1;
            }
            catch
            {
            }
            return result;
        }
        public abstract void Read();
        public abstract void Run();
    }
}