using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.server
{
    public abstract class SendPacket : IDisposable
    {
        public MemoryStream mstream = new MemoryStream();
        bool disposed = false;
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        public byte[] GetBytes(string name)
        {
            try
            {
                Write();
                return mstream.ToArray();
            }
            catch (Exception ex)
            {
                Logger.Error("GetBytes problem at: " + name + "\r\n" + ex.ToString());
                return new byte[0];
            }
        }
        public byte[] GetCompleteBytes(string name)
        {
            try
            {
                Write();
                byte[] data = mstream.ToArray();
                if (data.Length >= 2)
                {
                    ushort size = Convert.ToUInt16(data.Length - 2);
                    List<byte> list = new List<byte>(data.Length + 2);
                    list.AddRange(BitConverter.GetBytes(size));
                    list.AddRange(data);
                    return list.ToArray();
                }
                return new byte[0];
            }
            catch (Exception ex)
            {
                Logger.Error("GetCompleteBytes problem at: " + name + "\r\n" + ex.ToString());
                return new byte[0];
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            mstream.Dispose();
            if (disposing)
                handle.Dispose();
            disposed = true;
        }
        protected internal void WriteIP(string address)
        {
            WriteB(IPAddress.Parse(address).GetAddressBytes());
        }
        protected internal void WriteIP(IPAddress address)
        {
            WriteB(address.GetAddressBytes());
        }
        protected internal void WriteB(byte[] value)
        {
            mstream.Write(value, 0, value.Length);
        }
        protected internal void WriteB(byte[] value, int offset, int length)
        {
            mstream.Write(value, offset, length);
        }
        protected internal void WriteD(bool value)
        {
            WriteB(new byte[] { Convert.ToByte(value), 0, 0, 0 });
        }
        protected internal void WriteD(uint valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteD(int value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteH(ushort valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteH(short val)
        {
            WriteB(BitConverter.GetBytes(val));
        }
        protected internal void WriteC(byte value)
        {
            mstream.WriteByte(value);
        }
        /// <summary>
        /// True = 1; False = 0.
        /// </summary>
        /// <param name="value"></param>
        protected internal void WriteC(bool value)
        {
            mstream.WriteByte(Convert.ToByte(value));
        }
        protected internal void WriteT(float value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteF(double value)
        {
            WriteB(BitConverter.GetBytes(value));
        }
        protected internal void WriteQ(ulong valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        protected internal void WriteQ(long valor)
        {
            WriteB(BitConverter.GetBytes(valor));
        }
        /// <summary>
        /// Escreve um texto no formato Unicode.
        /// </summary>
        /// <param name="value">Texto</param>
        protected internal void WriteS(string value)
        {
            if (value != null)
                WriteB(Encoding.Unicode.GetBytes(value));
        }
        protected internal void WriteS(string name, int count)
        {
            if (name == null)
                return;
            WriteB(ConfigGB.EncodeText.GetBytes(name));
            WriteB(new byte[count - name.Length]);
        }
        protected internal void WriteS(string name, int count, int CodePage)
        {
            if (name == null)
                return;
            WriteB(Encoding.GetEncoding(CodePage).GetBytes(name));
            WriteB(new byte[count - name.Length]);
        }
        public abstract void Write();
    }
}