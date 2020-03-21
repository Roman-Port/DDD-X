using System;
using System.Collections.Generic;
using System.Text;

namespace DDDBotX.Framework.MessageDecoder.Payloads
{
    public class PlayerSwitchedClassEventPayload : DDDMessage
    {
        public override void Decode(byte[] data, DDDConnection conn)
        {
            player_guid = ReadIntString(data, 1);
            class_name = ReadString(data, 13, 32);
        }

        public int player_guid;
        public string class_name;
    }
}
