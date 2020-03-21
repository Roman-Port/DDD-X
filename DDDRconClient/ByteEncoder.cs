using System;
using System.Collections.Generic;
using System.Text;

namespace DDDRconClient
{
    static class ByteEncoder
    {
        public static int ReadInt32(byte[] buffer, int pos)
        {
            //Copy to internal buffer
            byte[] b = new byte[4];
            Array.Copy(buffer, pos, b, 0, 4);

            //Reverse if needed
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);

            //Read
            return BitConverter.ToInt32(b);
        }

        public static void WriteInt32(byte[] buffer, int pos, int data)
        {
            //Write to internal buffer
            byte[] b = BitConverter.GetBytes(data);

            //Reverse if needed
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(b);

            //Write
            Array.Copy(b, 0, buffer, pos, 4);
        }
    }
}
