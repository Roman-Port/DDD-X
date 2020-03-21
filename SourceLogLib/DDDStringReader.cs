using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RomanPort.SourceLogLib
{
    public class DDDStringReader
    {
        public MemoryStream sr;

        public DDDStringReader(byte[] s)
        {
            sr = new MemoryStream(s);
        }

        public int ReadPaddedInt()
        {
            //Read string
            string d = ReadConstString(4);
            return int.Parse(d);
        }

        public string ReadEmbeddedString()
        {
            //Read the length
            int len = ReadPaddedInt();

            //Read the string
            return ReadConstString(len);
        }

        public string ReadConstString(int len)
        {
            byte[] buf = new byte[len];
            sr.Read(buf, 0, len);
            return Encoding.UTF8.GetString(buf);
        }
    }
}
