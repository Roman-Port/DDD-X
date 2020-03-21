using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerExecCommandActionPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            player_guid = ReadIntString(data, 1);
            command = ReadString(data, 13, 256);
            args = ReadString(data, 269, 1024);
        }

        public int player_guid;
        public string command;
        public string args;
    }
}
