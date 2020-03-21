using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerChangeNamePayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            player_guid = ReadIntString(data, 1);
            old_name = ReadString(data, 13, 32);
            new_name = ReadString(data, 45, 32);
        }

        public int player_guid;
        public string old_name;
        public string new_name;
    }
}
