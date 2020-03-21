using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class ModAuthRequestPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            version_major = data[1];
            version_major = data[2];
            server_name = ReadString(data, 3, 32);
            map_name = ReadString(data, 35, 32);
        }

        public byte version_major;
        public byte version_minor;
        public string server_name;
        public string map_name;
    }
}
